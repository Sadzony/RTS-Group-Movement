using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupManager : MonoBehaviour
{
    //Singleton Template
    private static GroupManager instance;
    public static GroupManager Instance
    {
        get { return instance; }
        private set { instance = value; }
    }
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    //Member variables

    //Maps a squad to a list of groups. Unit references come from squad
    private Dictionary<Squad, HashSet<Group>> squadronMap = new Dictionary<Squad, HashSet<Group>>();

    //Maps units to its neighbours
    private Dictionary<Unit, HashSet<Unit>> neighbourMap = new Dictionary<Unit, HashSet<Unit>>();

    //Functions

    //Creates a neighbour map entry when the unit is created
    public void AddUnitEntry(Unit unit)
    {
        if(!neighbourMap.ContainsKey(unit))
        {
            neighbourMap.Add(unit, new HashSet<Unit>());
        }
    }
    //Called when unit dies
    public void RemoveUnitEntry(Unit unit)
    {
        //Remove unit from neighbour map of every neighbouring unit
        if(neighbourMap.TryGetValue(unit, out HashSet<Unit> neighbors))
        {
            foreach(Unit neighbor in neighbors)
            {
                if(neighbourMap.TryGetValue(neighbor, out HashSet<Unit> units))
                {
                    units.Remove(unit);
                }
            }
            neighbourMap.Remove(unit);
        }
    }

    //Called when squad created
    public void AddSquadronEntry(Squad squad)
    {
        if (!squadronMap.ContainsKey(squad))
        {
            squadronMap.Add(squad, new HashSet<Group>());
        }
    }
    //Called when squad is removed
    public void RemoveSquadronEntry(Squad squad)
    {
        if(squadronMap.TryGetValue(squad, out HashSet<Group> groups))
        {
            //dismantle groups

            //Remove squadron entry
            squadronMap.Remove(squad);
        }
    }
    public void Update()
    {
        //Iterate every squad and find the neighbours
        foreach (KeyValuePair<Squad, HashSet<Group>> squadronGroups in squadronMap)
        {
            //Get the squad's units
            HashSet<Unit> squadUnits = squadronGroups.Key.GetMembers();
            foreach (Unit unit in squadUnits)
            {
                //Update neighbour map for that unit

                //Check if unit exists in group set

                //if not, join to a neighbouring group, or create a new one
            }

        }

        //Neighbour map is now made

        //Iterate every squad and recalculate groups
        foreach (KeyValuePair<Squad, HashSet<Group>> squadronGroups in squadronMap)
        {
            //Iterate the set of groups

            //Check if any groups combine, based on the list of neighbours
        }
    }
}
