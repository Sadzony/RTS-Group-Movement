using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Group : MonoBehaviour
{
    [SerializeField] List<Unit> _members = new List<Unit>();
    private Squad owner;
    private HashSet<Unit> members = new HashSet<Unit>();

    //Maps units to its neighbours
    public Dictionary<Unit, HashSet<Unit>> neighbourMap = new Dictionary<Unit, HashSet<Unit>>();

    public Waypoint goalNode;
    public Vector3 goalCentre;
    public Vector3 groupCentre = Vector3.zero;
    public Dictionary<Unit, Waypoint> unitPaths = new Dictionary<Unit, Waypoint>();

    public HashSet<Unit> completedSet = new HashSet<Unit>();

    public float finishDistance = 0.0f;

    public Group()
    {
        members = new HashSet<Unit>();
        neighbourMap = new Dictionary<Unit, HashSet<Unit>>();
        
    }
    public int Count()
    {
        return members.Count;
    }
    public void SetOwner(Squad squad)
    {
        owner = squad;
        goalNode = squad.goalNode;
        goalCentre = goalNode.position;
    }
    public void Dismantle()
    {
        foreach(Unit unit in members)
        {
            GroupManager.Instance.GroupDisown(unit);
        }
        GroupManager.Instance.RemoveGroup(owner, this);
        if(gameObject != null)
            Destroy(gameObject);
    }
    public HashSet<Unit> GetNeighbours(Unit unit)
    {
        if (neighbourMap.TryGetValue(unit, out HashSet<Unit> output))
            return new HashSet<Unit>(output);
        return new HashSet<Unit>();
    }
    public void JoinGroup(Unit unit)
    {
        members.Add(unit);
        OnValidate();
        if (!neighbourMap.ContainsKey(unit))
        {
            neighbourMap.Add(unit, new HashSet<Unit>());
        }
        //If the unit doesn't have a path entry, then it must create a new path
        if(!unitPaths.ContainsKey(unit))
        {
            FindPath(unit);
        }
        if (!firstUnitAdded)
        { AddToFinishDistance(unit); firstUnitAdded = true; }
        GroupManager.Instance.GroupOwn(this, unit);
    }
    public void LeaveGroup(Unit unit)
    {
        members.Remove(unit);
        GroupManager.Instance.GroupDisown(unit);
        OnValidate();
        if (members.Count <= 0)
            Dismantle();

        //Remove neighbourmap entries from its neighbours
        if(neighbourMap.TryGetValue(unit, out HashSet<Unit> neighbours))
        {
            foreach(Unit neighbour in neighbours)
            {
                if(neighbourMap.TryGetValue(neighbour, out HashSet<Unit> otherNeighbours))
                {
                    otherNeighbours.Remove(unit);
                }
            }
        }
        neighbourMap.Remove(unit);
    }

    //Takes an other group and combines it into this one
    public Group CombineGroups(Group other)
    {
        //copy the other group data
        HashSet<Unit> otherMembers = new HashSet<Unit>(other.members);
        HashSet<Unit> otherCompleted = new HashSet<Unit>(other.completedSet);
        Dictionary<Unit, HashSet<Unit>> otherNeighbours = new Dictionary<Unit, HashSet<Unit>>(other.neighbourMap);
        Dictionary<Unit, Waypoint> otherPaths = new Dictionary<Unit, Waypoint>(other.unitPaths);

        if (other.finishDistance > this.finishDistance)
            finishDistance = other.finishDistance;

        //Dismantle other group
        other.Dismantle();

        //Combine the data
        foreach (Unit unit in otherMembers)
        {
            GroupManager.Instance.GroupOwn(this, unit);
            members.Add(unit);
            if (otherNeighbours.TryGetValue(unit, out HashSet<Unit> neighbours))
                neighbourMap.Add(unit, neighbours);
            if(otherPaths.TryGetValue(unit, out Waypoint node))
                unitPaths.Add(unit, node);
            if (otherCompleted.Contains(unit))
            {
                completedSet.Add(unit);
                AddToFinishDistance(unit);
            }
        }
        UpdateGoalCentre();
        OnValidate();

        return this;
    }
    public void FindPath(Unit unit)
    {
        //LoS to goal - if in sight, then no need to find path
        NavMeshHit hit;
        if (!NavMesh.Raycast(unit.transform.position, goalNode.position, out hit, NavMesh.AllAreas))
        {
            //in sight, set the goal node as current node
            if(!unitPaths.ContainsKey(unit))
                unitPaths.Add(unit, goalNode);
            else
                unitPaths[unit] = goalNode;
        }
        else
        {
            NavMeshPath outputPath = new NavMeshPath();
            NavMesh.CalculatePath(unit.agent.nextPosition, goalNode.position, NavMesh.AllAreas, outputPath);
            if (!unitPaths.ContainsKey(unit))
                unitPaths.Add(unit, ConvertToWaypoints(outputPath));
            else
                unitPaths[unit] = ConvertToWaypoints(outputPath);
        }
    }
    private Waypoint ConvertToWaypoints(NavMeshPath path)
    {
        Waypoint currentWaypoint = goalNode;

        //Start from goal. Ignore first and last corner. First is the unit's position and last is the goal position.

        //Iterate the  points and create waypoints
        if (path.corners.Count() > 2)
        {
            for (int j = path.corners.Length - 2; j > 0; j--)
            {

                Vector3 position = path.corners[j];
                Waypoint pathPoint = new Waypoint(path.corners[j], position);
                pathPoint.next = currentWaypoint;
                currentWaypoint = pathPoint;
            }
        }

        return currentWaypoint;
    }
    public void PopAll(Unit unit)
    {
        if (unitPaths.TryGetValue(unit, out Waypoint node))
        {
            unitPaths[unit] = null;
            completedSet.Add(unit);
            AddToFinishDistance(unit);
            unit.CompleteCommand();
        }
    }
    public void PopWaypoint(Unit unit)
    {
        if(unitPaths.TryGetValue(unit, out Waypoint node))
        {
            if (node.next != null)
                unitPaths[unit] = node.next;
            if(node == goalNode)
            {
                completedSet.Add(unit);
                AddToFinishDistance(unit);
                unit.CompleteCommand();
                unitPaths[unit] = null;
            }
        }
    }
    bool firstUnitAdded = false;
    float totalUnitArea = 0.0f;
    void AddToFinishDistance(Unit unit)
    {
        //The distance is the radius of the sum of areas * multiplier
        float unitArea = Mathf.PI * unit.agent.radius * unit.agent.radius;
        totalUnitArea += unitArea * SteeringData.Instance.goalAreaGrowthMultiplier;
        finishDistance = Mathf.Sqrt(totalUnitArea/Mathf.PI);
        UpdateGoalCentre();
    }
    void UpdateGoalCentre()
    {
        //If there are units in the completed set, find their mean position
        if (completedSet.Count > 0)
        {
            Vector3 meanPos = goalNode.position;
            foreach (Unit u in completedSet)
                meanPos += u.agent.nextPosition;
            meanPos /= completedSet.Count + 1;
            goalCentre = meanPos;
        }
        //otherwise use goal node position
        else
            goalCentre = goalNode.position;
    }
    private void OnValidate()
    {
        _members = members.ToList();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Vector3 meanPos = Vector3.zero;
        //foreach(Unit u in members)
        //{
        //    meanPos+=u.agent.nextPosition;
        //}
        //meanPos/= members.Count;
        //groupCentre = meanPos + new Vector3(0, 0.5f, 0);
    }
}
