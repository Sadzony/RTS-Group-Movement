using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
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
    public HashSet<Waypoint> leaves = new HashSet<Waypoint>();
    public Dictionary<Unit, Waypoint> unitPaths = new Dictionary<Unit, Waypoint>();

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
    }
    public void Dismantle()
    {
        foreach(Unit unit in members)
        {
            GroupManager.Instance.GroupDisown(unit);
        }
        GroupManager.Instance.RemoveGroup(owner, this);
        Destroy(gameObject);
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
        Dictionary<Unit, HashSet<Unit>> otherNeighbours = new Dictionary<Unit, HashSet<Unit>>(other.neighbourMap);
        Dictionary<Unit, Waypoint> otherPaths = new Dictionary<Unit, Waypoint>(other.unitPaths);

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
        }
        OnValidate();

        return this;
    }
    private void FindPath(Unit unit)
    {
        //LoS to goal - if in sight, then no need to find path
        NavMeshHit hit;
        if (!NavMesh.Raycast(unit.transform.position, goalNode.position, out hit, NavMesh.AllAreas))
        {
            //in sight, set the goal node as current node
            unitPaths.Add(unit, goalNode);
        }
        //If leaves exist, then a valid path might already be present
        else if (leaves.Count > 0)
        {
            HashSet<Waypoint> exploredNodes = new HashSet<Waypoint>();
            HashSet<Waypoint> searchSet = new HashSet<Waypoint>(leaves);
            bool match = false;
            Waypoint outputNode = null;
            while(match == false)
            {
                //if the search set is empty, exit while loop
                if (searchSet.Count <= 0)
                    break;
                List<Waypoint> searchList = searchSet.ToList();
                foreach (Waypoint node in searchList)
                {
                    //If the node was already explored, then the branch can be removed from the search set
                    if (exploredNodes.Contains(node) || node == goalNode)
                    {
                        searchSet.Remove(node);
                        continue;
                    }
                    exploredNodes.Add(node);
                    //LoS check to the path node
                    if (!NavMesh.Raycast(unit.transform.position, node.position, out hit, NavMesh.AllAreas))
                    {
                        outputNode = node;
                        match = true;
                        break;
                    }

                    //Remove the current node at the search set and replace it with the next node in path
                    searchSet.Remove(node);
                    searchSet.Add(node.next);
                }
            }
            //If there was a match, continue down that branch until cannot see the next waypoint
            if(match)
            {
                bool seeNext = true;
                while (seeNext)
                {
                    Waypoint next = outputNode.next;
                    if (NavMesh.Raycast(unit.transform.position, next.position, out hit, NavMesh.AllAreas))
                    {
                        seeNext = false;
                        break;
                    }
                    outputNode = next;
                }
                unitPaths.Add(unit, outputNode);
            }
            //if there was no match, find path
            else
            {
                NavMeshPath outputPath = new NavMeshPath();
                NavMesh.CalculatePath(unit.transform.position, goalNode.position, NavMesh.AllAreas, outputPath);
                unitPaths.Add(unit, ConvertToWaypoints(outputPath));
            }

        }
        //If there are no leaves, then the unit finds the first path
        else
        {
            NavMeshPath outputPath = new NavMeshPath();
            NavMesh.CalculatePath(unit.transform.position, goalNode.position, NavMesh.AllAreas, outputPath);
            unitPaths.Add(unit, ConvertToWaypoints(outputPath));
        }
    }
    private Waypoint ConvertToWaypoints(NavMeshPath path)
    {
        Waypoint currentWaypoint = goalNode;
        //Finding the waypoint at which the path branches from the existing paths
        //Start from goal. Ignore first and last corner. First is the unit's position and last is the goal position.
        int i;
        for(i = path.corners.Length -1; i > 0; i--)
        {
            bool match = false;
            //Check the branches at current waypoint
            foreach(Waypoint branch in currentWaypoint.branches)
            {
                //If the path corner matches a branch then they're the same point, so continue search
                if (branch.isEqual(path.corners[i]))
                {
                    match = true;
                    currentWaypoint = branch;
                    //If this is the last corner of the path, and it matches an existing branch, then the path is the same as existing path
                    if (i == 1)
                        return branch;
                    break;
                }
            }
            //If the search didn't match any branches, then the branch is created at currentWaypoint
            if(!match)
                break;
        }
        //Iterate the remaining points and create waypoints
        for (int j = i; j > 0; j--)
        {
            Waypoint pathPoint = new Waypoint(path.corners[j]);
            pathPoint.next = currentWaypoint;
            currentWaypoint.branches.Add(pathPoint);
            //Update leaves
            if(leaves.Contains(currentWaypoint))
            {
                leaves.Remove(currentWaypoint);
                leaves.Add(pathPoint);
            }
            currentWaypoint = pathPoint;
        }
        if(!leaves.Contains(currentWaypoint))
            leaves.Add(currentWaypoint);

        return currentWaypoint;
    }
    public void PopWaypoint(Unit unit)
    {
        if(unitPaths.TryGetValue(unit, out Waypoint node))
        {
            if (node.next != null)
                unitPaths[unit] = node.next;
        }
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
        
    }
}
