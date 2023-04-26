using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    [HideInInspector] public Collider selectionCollider;
    [HideInInspector] public bool isSelected;
    [HideInInspector] public NavMeshAgent agent;

    [HideInInspector] public bool standby = true;
    [HideInInspector] public bool isMoving = false;
    

    public Vector3 velocity = Vector3.zero;


    private GameObject selectionSprite;

    [HideInInspector] public LineRenderer velocityDebug;
    [HideInInspector] public LineRenderer neighbourhoodDebug;
    [HideInInspector] public LineRenderer pathDebug;
    [HideInInspector] public LineRenderer goalDebug;
    [HideInInspector] public LineRenderer commandLine;

    private SteeringData data;

    // Start is called before the first frame update
    void Start()
    {
        SelectionManager.Instance.AddToSelectable(this);
        agent = GetComponent<NavMeshAgent>();
        selectionSprite = this.gameObject.transform.Find("SelectionSprite").gameObject;
        selectionCollider = this.gameObject.transform.Find("SelectionCollider").GetComponent<Collider>();

        velocityDebug = transform.Find("Debug Velocity").GetComponent<LineRenderer>();
        neighbourhoodDebug = transform.Find("Debug Neighbourhood").GetComponent<LineRenderer>();
        pathDebug = transform.Find("Debug Path").GetComponent<LineRenderer>();
        goalDebug = transform.Find("Debug Goal").GetComponent<LineRenderer>();
        commandLine = transform.Find("Command Path").GetComponent<LineRenderer>();

        data = SteeringData.Instance;
    }
    public void Die()
    {
        Transform mesh = transform.Find("HPCharacter");
        mesh.GetComponent<AgentAnimationController>().enabled = false;
        mesh.GetComponent<Animator>().SetTrigger("death");

        SelectionManager.Instance.Deselect(this);
        SelectionManager.Instance.RemoveFromSelectable(this);

        velocityDebug.gameObject.SetActive(false);
        neighbourhoodDebug.gameObject.SetActive(false);
        pathDebug.gameObject.SetActive(false);
        goalDebug.gameObject.SetActive(false);
        commandLine.gameObject.SetActive(false);

        GetComponent<NavMeshAgent>().enabled = false;

        StartCoroutine(DeleteAfterDeath());
    }
    IEnumerator DeleteAfterDeath()
    {
        yield return new WaitForSeconds(3);
        Destroy(this.gameObject);
    }

    public void OnSelected()
    {
        isSelected = true;
        selectionSprite.SetActive(true);
    }
    public void OnDeselected()
    {
        isSelected = false;
        selectionSprite.SetActive(false);
    }
    public void OnCommandCancel()
    {
        standby = true;
    }
    public void ReceiveCommand()
    {
#nullable enable
        //try getting the next command. If successful, receive it. If the unit is not in standby, it's ignored
        if (standby == true)
        {
            CommandManager.Instance.OnCommandReceived(this);
            if (CommandManager.Instance.GetCurrentCommand(this, out Command? command))
                standby = false;
        }
#nullable disable
    }
    public void CompleteCommand()
    {
        //Complete the command
        CommandManager.Instance.OnCommandCompleted(this);
        //Since the command is completed, the unit is waiting for the next one, so enter standby mode.
        standby = true;

        //If there are more commands, receive the next one
        if (CommandManager.Instance.NextCommandAvailable(this))
            ReceiveCommand();
    }
    private bool CrossedWaypoint(Waypoint waypoint, Group group)
    {
        Vector3 unitPos = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 goalPos = new Vector3(group.goalNode.position.x, 0, group.goalNode.position.z);
        Vector3 goalCentrePos = new Vector3(group.goalCentre.x, 0, group.goalCentre.z);
        Vector3 toGoalCentre = goalCentrePos - unitPos;
        Vector3 toGoal = goalPos - unitPos;



        if (waypoint != group.goalNode)
        {

            //Check if the unit is within goal distance, and that one of its neighbours has also already completed the route

            if (toGoalCentre.magnitude < group.finishDistance)
            {
                if (data.finishMessagingEnabled)
                {
                    if (group.neighbourMap.TryGetValue(this, out HashSet<Unit> neighbours))
                    {
                        foreach (Unit n in neighbours)
                        {
                            if (group.completedSet.Contains(n))
                            {
                                group.PopAll(this);
                                return false;
                            }
                        }
                    }
                }
                if(velocity.magnitude < data.goalReachedSpeed)
                {
                    group.PopAll(this);
                    return false;
                }    
            }
            Vector3 waypointPos = new Vector3(waypoint.position.x, 0, waypoint.position.z);
            Vector3 nextPos = new Vector3(waypoint.next.position.x, 0, waypoint.next.position.z);

            //Get the direction to the waypoint and to the next node
            Vector3 toWaypoint = waypointPos - unitPos;
            Vector3 toNext = nextPos - waypointPos;

            float dot = Vector3.Dot(toWaypoint.normalized, toNext.normalized);



            NavMeshHit hit;

            //if the waypoint is very close, mark as completed
            if (toWaypoint.magnitude < data.waypointNearDistance)
                return true;
            //if LoS to next, then can start walking to next
            //else if (NavMesh.Raycast(agent.nextPosition, waypoint.next.position, out hit, NavMesh.AllAreas) == false)
            //    return true;

            //Find if the waypoint is behind the unit
            else if (dot <= 0)
            {
                //Find if the unit has LoS to the current waypoint
                if (NavMesh.Raycast(agent.nextPosition, waypoint.position, out hit, NavMesh.AllAreas) == false)
                    return true;
            }

        }
        else if(waypoint == group.goalNode)
        {
            if (toGoal.magnitude < group.finishDistance || toGoalCentre.magnitude < group.finishDistance)
            {
                //Find closest wall
                NavMeshHit hit;
                if (NavMesh.Raycast(agent.nextPosition, agent.nextPosition + velocity, out hit, NavMesh.AllAreas))
                {
                    //If the raycast hits then a collision is on course. Truncate the speed to the distance to obstacle

                    Vector3 toHit = hit.position - agent.nextPosition;
                    //Remove the Y difference
                    toHit = new Vector3(toHit.x, 0, toHit.z);
                    velocity = Vector3.ClampMagnitude(velocity, toHit.magnitude);
                    
                }

                //check if velocity is close to 0
                if (velocity.magnitude < data.goalReachedSpeed)
                {
                    return true;
                }
                else if (toGoal.magnitude < data.waypointNearDistance)
                    return true;
            }
        }
        return false;
    }
    public HashSet<Unit> FindLocalNeighbours(HashSet<Unit> groupNeighbours)
    {
        HashSet<Unit> outputSet = new HashSet<Unit>();
        foreach(Unit groupNeighbour in groupNeighbours)
        {

            Vector3 toOther = new Vector3(groupNeighbour.transform.position.x, 0, groupNeighbour.transform.position.z)
                              - new Vector3(transform.position.x, 0, transform.position.z);
            float distance = toOther.magnitude;
            toOther.Normalize();
            //float angle = Vector3.Angle(transform.forward, toOther);

            //Find the angle between the neighbours
            float angle = Mathf.Acos(Mathf.Clamp(Vector3.Dot(transform.forward, toOther), -1, 1));
            angle = angle * Mathf.Rad2Deg;

            //Compare the angle to half angle of the FoV
            if (angle <= data.neighbourLoSArc / 2)
                outputSet.Add(groupNeighbour);
        }
        return outputSet;
    }
    private void Update()
    {
        //Check if the unit belongs to a group
        GroupManager groupManager = GroupManager.Instance;
#nullable enable
        Group? currentGroup = groupManager.GetGroup(this);
        Waypoint? currentNode = null;
        Vector3 unitPosition = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 nodePosition = Vector3.zero;
#nullable disable
        if (currentGroup != null)
        {
            //check if the unit is in the completed set. If not, ensure the path is valid.
            if(!currentGroup.completedSet.Contains(this))
            {
                if (currentGroup.unitPaths.TryGetValue(this, out currentNode) && currentNode != null)
                {
                    //Check if path is still valid, by casting for LoS to current node
                    NavMeshHit hit;
                    if (NavMesh.Raycast(agent.nextPosition, currentNode.position, out hit, NavMesh.AllAreas) == true)
                    {
                        //If agent cannot see the current node, then the path is invalid and should be recalculated.
                        currentGroup.FindPath(this);
                    }
                }
                // if the agent does hasn't finished, but doesn't have a path then a new one should be generated.
                else
                {
                    currentGroup.FindPath(this);
                }
            }
            //If the unit belongs to a group then a path was generated. Follow the next waypoint in path
            if(currentGroup.unitPaths.TryGetValue(this, out currentNode) && currentNode != null)
            {


                nodePosition = new Vector3(currentNode.position.x, 0, currentNode.position.z);
                isMoving = true;

                //Calculate seek - difference in velocity to a desired velocity, which leads to target
                Vector3 toNode = nodePosition - unitPosition;
                Vector3 desiredVelocity = toNode.normalized * data.unitSpeed;
                Vector3 seek = desiredVelocity - velocity;
                

                HashSet<Unit> neighbours = currentGroup.GetNeighbours(this);

                if(data.neighbourLoSArcEnabled)
                    neighbours = FindLocalNeighbours(neighbours);

                //Calculate the flocking forces
                Vector3 cohesion = Vector3.zero;
                Vector3 separation = Vector3.zero;
                Vector3 alignment = Vector3.zero;
                int a = 0;
                foreach(Unit n in neighbours)
                {
                    Vector3 neighbourPos = new Vector3(n.transform.position.x, 0, n.transform.position.z);
                    Vector3 fromNeighbour = unitPosition - neighbourPos;
                    Vector3 toNeighbour = neighbourPos - unitPosition;
                    float distance = fromNeighbour.magnitude;
                    if (distance < data.cohesionDistance)
                        //cohesion increases in weight the further the unit is.
                    {
                        cohesion += (toNeighbour.normalized * data.unitSpeed) * (distance/data.cohesionDistance); 
                    }
                    if (distance < data.separationDistance)
                        //separation decreases in weight the further the unit is
                    { separation += (fromNeighbour.normalized * data.unitSpeed) * ((data.separationDistance - distance) / data.separationDistance); }
                    if (distance < data.alignmentDistance)
                    { alignment += n.velocity; a++; }
                }
                if (a > 0)
                {
                    //Alignment is the difference in velocity, towards mean velocity
                    alignment /= a;
                    alignment = alignment - velocity;
                }

                //Separation will act together with the seek to create a steering force 
                Vector3 steering = ((separation * data.separationWeight) + (seek * data.seekWeight));

                //Alignment acts together with cohesion to create flocking force
                Vector3 flocking = ((cohesion  * data.cohesionWeight) + (alignment * data.alignmentWeight)) * data.flockingPower;

                //Truncate the result to unit speed
                velocity = Vector3.ClampMagnitude(velocity + steering + flocking, data.unitSpeed);

                //avoidance behaviour
                Vector3 avoidance = FindAvoidance();

                //Truncate again
                velocity = Vector3.ClampMagnitude(velocity + avoidance, data.unitSpeed);

                //Arrive behaviour
                if(data.arriveEnabled && currentNode == currentGroup.goalNode)
                {
                    //Truncate to target steering goal
                    velocity = Vector3.ClampMagnitude(velocity, toNode.magnitude);
                }
            }
            else
            {
                isMoving = false;
                velocity = Vector3.zero;
            }
        }
        else
        {
            isMoving = false;
            velocity = Vector3.zero;
        }
        //calculate next rotation and position
        if (velocity != Vector3.zero)
        {
            Quaternion goalOrientation = Quaternion.LookRotation(velocity.normalized);
            //transform.rotation = Quaternion.Slerp(transform.rotation, goalOrientation, data.unitRotationSpeed * Time.deltaTime);
            //transform.rotation = Quaternion.RotateTowards(transform.rotation, goalOrientation, data.unitRotationSpeed * Time.deltaTime);
            transform.rotation = Util.SmoothDamp(transform.rotation, goalOrientation, ref rotationRef, data.unitRotationTime);
            Vector3 movement = velocity * Time.deltaTime;
            agent.Move(movement);
        }
        if (currentGroup != null && currentNode != null && CrossedWaypoint(currentNode, currentGroup))
        {
            currentGroup.PopWaypoint(this);
        }
            
    }
    Quaternion rotationRef;
    Vector3 FindAvoidance()
    {
        Vector3 output = Vector3.zero;
        
        NavMeshHit hit;
        //Send raycast in direction and magnitude of velocity
        if(NavMesh.Raycast(agent.nextPosition, agent.nextPosition + velocity, out hit, NavMesh.AllAreas))
        {
            //If the raycast hits then a collision is on course. Apply a force in direction of normal, and magnitude of penetration distance

            Vector3 toHit = hit.position - agent.nextPosition;
            //Remove the Y difference
            toHit = new Vector3(toHit.x, 0, toHit.z);
            float magnitude = (toHit - velocity).magnitude;
            output += hit.normal * magnitude;
        }
        return output;
    }
}
