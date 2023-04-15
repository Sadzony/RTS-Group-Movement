using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    [HideInInspector] public Collider selectionCollider;
    [HideInInspector] public bool isSelected;
    [HideInInspector] public NavMeshAgent agent;

    [HideInInspector] public bool standby = true;
    [HideInInspector] public bool isMoving = false;

    public float target_speed;
    public float max_steering;
    public float rotation_speed;

    public Vector3 velocity = Vector3.zero;
    public float neighbourLoSArc;

    private GameObject selectionSprite;

    // Start is called before the first frame update
    void Start()
    {
        SelectionManager.Instance.AddToSelectable(this);
        agent = GetComponent<NavMeshAgent>();
        selectionSprite = this.gameObject.transform.Find("SelectionSprite").gameObject;
        selectionCollider = this.gameObject.transform.Find("SelectionCollider").GetComponent<Collider>();
    }
    public void Die()
    {
        Transform mesh = transform.Find("HPCharacter");
        mesh.GetComponent<AgentAnimationController>().enabled = false;
        mesh.GetComponent<Animator>().SetTrigger("death");

        SelectionManager.Instance.Deselect(this);
        SelectionManager.Instance.RemoveFromSelectable(this);


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
            {
                standby = false;
            }
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
        {
            ReceiveCommand();
        }
    }
    private bool CrossedWaypoint(Waypoint waypoint, Group group)
    {
        if(waypoint != group.goalNode)
        {
            //Get the direction to the waypoint and to the next node
            Vector3 toWaypoint = waypoint.position - transform.position;
            Vector3 toNext = waypoint.next.position - waypoint.position;
            float dot = Vector3.Dot(toWaypoint, toNext);
            //Find if the waypoint is behind the unit
            if(dot < 0)
            {
                //Find if the unit has LoS to the waypoint
                NavMeshHit hit;
                if (!NavMesh.Raycast(transform.position, waypoint.position, out hit, NavMesh.AllAreas))
                {
                    return true;
                }
                //If there is no LoS to the waypoint, check if the waypoint is close
                else
                {
                    if (toWaypoint.sqrMagnitude < 1)
                        return true;
                }
            }
        }
        return false;
    }
    private void Update()
    {
        //Check if the unit belongs to a group
        GroupManager groupManager = GroupManager.Instance;
#nullable enable
        Group? currentGroup = groupManager.GetGroup(this);
        Waypoint? currentNode = null;
#nullable disable
        if (currentGroup != null)
        {
            //If the unit belongs to a group then a path was generated. Follow the next waypoint in path
            if(currentGroup.unitPaths.TryGetValue(this, out currentNode))
            {
                isMoving = true;
                Vector3 desiredVelocity = currentNode.position - this.transform.position;

                desiredVelocity = desiredVelocity.normalized * target_speed;
                Vector3 steeringPower = desiredVelocity - velocity;
                velocity = Vector3.ClampMagnitude(velocity + steeringPower, target_speed);
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
        if(velocity != Vector3.zero)
        {
            Vector3 lookDirection = Vector3.RotateTowards(transform.forward, velocity.normalized, rotation_speed * Time.deltaTime, 0.0f);
            transform.rotation = Quaternion.LookRotation(lookDirection);
            agent.Move(velocity * Time.deltaTime);
            if(currentGroup != null && currentNode != null && CrossedWaypoint(currentNode, currentGroup))
            {
                currentGroup.PopWaypoint(this);
            }
        }
    }
}
