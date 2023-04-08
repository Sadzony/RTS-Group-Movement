using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Group : MonoBehaviour
{
    private Squad owner;
    private HashSet<Unit> members = new HashSet<Unit>();

    //Maps units to its neighbours
    public Dictionary<Unit, HashSet<Unit>> neighbourMap = new Dictionary<Unit, HashSet<Unit>>();
    public void JoinGroup(Unit unit)
    {
        members.Add(unit);
        if(!neighbourMap.ContainsKey(unit))
        {
            neighbourMap.Add(unit, new HashSet<Unit>());
        }
    }
    public void LeaveGroup(Unit unit)
    {
        members.Remove(unit);

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
        return this;
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
