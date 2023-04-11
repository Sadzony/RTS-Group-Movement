using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


public class Group : MonoBehaviour
{
    [SerializeField] List<Unit> _members = new List<Unit>();
    private Squad owner;
    private HashSet<Unit> members = new HashSet<Unit>();

    //Maps units to its neighbours
    public Dictionary<Unit, HashSet<Unit>> neighbourMap = new Dictionary<Unit, HashSet<Unit>>();

    public Group()
    {
        members = new HashSet<Unit>();
        neighbourMap = new Dictionary<Unit, HashSet<Unit>>();
    }
    public void SetOwner(Squad squad)
    {
        owner = squad;
    }
    public void Dismantle()
    {
        foreach(Unit unit in members)
        {
            GroupManager.Instance.GroupDisown(unit);
        }
        OnValidate();
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
        //Iterate other groups members
        HashSet<Unit> otherMembers = new HashSet<Unit>(other.members);
        foreach(Unit unit in otherMembers)
        {
            other.members.Remove(unit);
            GroupManager.Instance.GroupOwn(this, unit);
            members.Add(unit);
            if(other.neighbourMap.TryGetValue(unit, out HashSet<Unit> neighbours))
            {
                neighbourMap.Add(unit, neighbours);
            }
        }

        //Dismantle other group
        other.Dismantle();


        OnValidate();

        return this;
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
