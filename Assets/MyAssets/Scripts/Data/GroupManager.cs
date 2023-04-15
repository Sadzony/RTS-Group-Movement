using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

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


    //Member variabless

    [SerializeField, Range(1, 20)] private float neighbourDistance;
    private float neighbourSquaredDistance;

    //Maps a squad to a list of groups. Unit references come from squad
    private Dictionary<Squad, HashSet<Group>> squadronMap = new Dictionary<Squad, HashSet<Group>>();

    //Maps a unit to a group that owns it
    private Dictionary<Unit, Group> groupOwnershipMap = new Dictionary<Unit, Group>();

    //Functions

    //Whenever a unit joins a particular squad, it begins being managed.
    public void ManageUnit(Unit unit)
    {
        if(!groupOwnershipMap.ContainsKey(unit))
        {
            groupOwnershipMap.Add(unit, null);
        }
    }
    //Whenever a unit leaves a particular squad, its also leaves the group manager
    public void UnmanageUnit(Unit unit)
    {
        if(groupOwnershipMap.TryGetValue(unit, out Group group) && group != null)
        {
            group.LeaveGroup(unit);
            groupOwnershipMap.Remove(unit);
        }
    }
    public void GroupOwn(Group group, Unit unit)
    {
        if(groupOwnershipMap.ContainsKey(unit))
        {
            groupOwnershipMap[unit] = group;
        }
    }
    public void GroupDisown(Unit unit)
    {
        if(groupOwnershipMap.ContainsKey(unit))
        {
            groupOwnershipMap[unit] = null;
        }
    }
#nullable enable
    public Group? GetGroup(Unit unit)
    {
        if(groupOwnershipMap.ContainsKey(unit))
        {
            return groupOwnershipMap[unit];
        }
        return null;
    }
#nullable disable
    public void RemoveGroup(Squad owner, Group group)
    {
        if(squadronMap.TryGetValue(owner, out HashSet<Group> squadGroups))
        {
            squadGroups.Remove(group);
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
            foreach(Group group in groups)
            {
                group.Dismantle();
            }

            //Remove squadron entry
            squadronMap.Remove(squad);
        }
    }
    //Called to recalculate inspector values
    private void OnValidate()
    {
        neighbourSquaredDistance = neighbourDistance * neighbourDistance;
    }
    private void Update()
    {
        //Iterate every squad and find the groups within it
        List<Squad> squadList = squadronMap.Keys.ToList();
        foreach (Squad squad in squadList)
        {
            //Update the groups for this squad
            if (squad.Count() != 0)
            {
                squadronMap[squad] = CalculateGroups(squad);
            }
                
        }
    }

    //Finds neighbouring units and creates group entries for them
    private HashSet<Group> CalculateGroups(Squad inputSquad)
    {
        //Get the squad's units
        HashSet<Unit> squadUnits = inputSquad.GetMembers();

        //Each unit keeps track of which units were already inspected
        Dictionary<Unit, HashSet<Unit>> solvedUnitMap = squadUnits.ToDictionary(x => x, x => new HashSet<Unit>());

        HashSet<Group> outputGroups = new HashSet<Group>();

        //Iterate the unit list
        foreach (Unit unit in squadUnits)
        {
            //Find the group for this unit
            Group group = FindNeighbouringGroup(unit, inputSquad, ref solvedUnitMap);

            outputGroups.Add(group);
        }
        return outputGroups;

    }
    //Finds the unit's neighbours and outputs the group that it belongs in
    private Group FindNeighbouringGroup(Unit unit, Squad inputSquad, ref Dictionary<Unit, HashSet<Unit>> solvedUnitMap)
    {
        //Get squad's units
        HashSet<Unit> squadUnits = inputSquad.GetMembers();

        //This will update the neighbour set for this group
        HashSet<Unit> neighbours = new HashSet<Unit>();

        Group outputGroup;
        bool neighbourFound = false;

        //Try to check the unit's entry at the solvedUnits map
        if(solvedUnitMap.TryGetValue(unit, out HashSet<Unit> solvedUnits) && solvedUnits.Count != 0)
        {
            foreach(Unit other in squadUnits)
            {
                if (other != unit)
                {
                    //if the other unit doesn't exist in this unit's solution set, then their distances haven't been calculated yet
                    if (!solvedUnits.Contains(other))
                    {
                        if (isGroup(unit, other))
                        {
                            //if match, find the group object for this and other neighbour. Since they're neighbours, they will share a group
                            Group neighbouringGroup = FindGroup(unit, other, inputSquad);
                            neighbourFound = true;
                            outputGroup = neighbouringGroup;
                            //Add the neighbouring unit to this unit's neighbour map
                            if (neighbouringGroup.neighbourMap.TryGetValue(unit, out HashSet<Unit> thisNeighbourMap))
                                thisNeighbourMap.Add(other);
                            //Add this unit to other units neighbour map
                            if (neighbouringGroup.neighbourMap.TryGetValue(other, out HashSet<Unit> otherNeighbourMap))
                                otherNeighbourMap.Add(unit);
                        }
                        else
                        {
                            //Try remove the neighbouring unit from this unit's neighbour map
                            if (groupOwnershipMap[unit] != null && groupOwnershipMap[unit].neighbourMap.TryGetValue(unit, out HashSet<Unit> thisNeighbourMap))
                                thisNeighbourMap.Remove(other);
                            //Try remove this unit from other units neighbour map
                            if (groupOwnershipMap[other] != null && groupOwnershipMap[other].neighbourMap.TryGetValue(other, out HashSet<Unit> otherNeighbourMap))
                                otherNeighbourMap.Remove(unit);
                        }
                        //This unit has been solved for other neighbour
                        solvedUnitMap[other].Add(unit);
                        //other neighbour has been solved for this unit
                        solvedUnitMap[unit].Add(other);
                    }
                }
            }
        }

        //If the unit doesn't exist exist in the solution set,
        //then this is the first unit and must be tested with every other unit
        else
        {
            foreach(Unit other in squadUnits)
            {
                if (other != unit)
                {
                    if (isGroup(unit, other))
                    {
                        //if match, find the group object for this and other neighbour. Since they're neighbours, they will share a group
                        Group neighbouringGroup = FindGroup(unit, other, inputSquad);
                        neighbourFound = true;
                        outputGroup = neighbouringGroup;
                        //Add the neighbouring unit to this unit's neighbour map
                        if (neighbouringGroup.neighbourMap.TryGetValue(unit, out HashSet<Unit> thisNeighbourMap))
                            thisNeighbourMap.Add(other);
                        //Add this unit to other units neighbour map
                        if (neighbouringGroup.neighbourMap.TryGetValue(other, out HashSet<Unit> otherNeighbourMap))
                            otherNeighbourMap.Add(unit);
                    }
                    else
                    {
                        //Try remove the neighbouring unit from this unit's neighbour map
                        if (groupOwnershipMap[unit] != null && groupOwnershipMap[unit].neighbourMap.TryGetValue(unit, out HashSet<Unit> thisNeighbourMap))
                            thisNeighbourMap.Remove(other);
                        //Try remove this unit from other units neighbour map
                        if (groupOwnershipMap[other] != null && groupOwnershipMap[other].neighbourMap.TryGetValue(other, out HashSet<Unit> otherNeighbourMap))
                            otherNeighbourMap.Remove(unit);
                    }
                    //This unit has been solved for other neighbour
                    solvedUnitMap[other].Add(unit);
                    //other neighbour has been solved for this unit
                    solvedUnitMap[unit].Add(other);
                }
            }
        }
        if(!neighbourFound && groupOwnershipMap[unit] == null)
        {
            //Make a new group object and add this unit
            GameObject newGroupObject = new GameObject("Group");
            Group newGroup = newGroupObject.AddComponent<Group>() as Group;
            newGroup.SetOwner(inputSquad);
            newGroup.JoinGroup(unit);
        }
        return groupOwnershipMap[unit];
    }

    //Called each time a neighbour is found. Returns the group that it belongs in
    private Group FindGroup(Unit unit, Unit neighbour, Squad inputSquad)
    {
        //Check the ownership dictionary for this and the other neighbour
        bool unitGroupFound = groupOwnershipMap.TryGetValue(unit, out Group unitGroup) && unitGroup != null;
        bool neighbourGroupFound = groupOwnershipMap.TryGetValue(neighbour, out Group neighbourGroup) && neighbourGroup != null;
        
        //If both have groups
        if(unitGroupFound && neighbourGroupFound)
        {
            //If the groups are different, the groups combine
            if (unitGroup != neighbourGroup)
            {
                if (unitGroup.Count() >= neighbourGroup.Count())
                {
                    unitGroup.CombineGroups(neighbourGroup);
                    return unitGroup;
                }
                    
                else
                {
                    neighbourGroup.CombineGroups(unitGroup);
                    return neighbourGroup;
                }
            }
            //otherwise, no changes
            return unitGroup;
        }
        //If neighbour doesn't have a group, but this unit does
        else if(unitGroupFound && !neighbourGroupFound)
        {
            unitGroup.JoinGroup(neighbour);
            return unitGroup;
        }
        //If this unit doesn't have a group, but neighbour does
        else if(!unitGroupFound && neighbourGroupFound)
        {
            neighbourGroup.JoinGroup(unit);
            return neighbourGroup;
        }
        //If neither have groups
        else
        {
            //Make a new group and add both units
            GameObject newGroupObject = new GameObject("Group");
            Group newGroup = newGroupObject.AddComponent<Group>() as Group;
            newGroup.SetOwner(inputSquad);
            newGroup.JoinGroup(unit);
            newGroup.JoinGroup(neighbour);
            return newGroup;
        }

    }
    //Finds whether 2 members are in distance to be in the same group
    private bool isGroup(Unit unit1, Unit unit2)
    {
        //compare horizontal distance
        Vector3 dir = new Vector3(unit1.transform.position.x, 0, unit1.transform.position.z) - 
                      new Vector3(unit2.transform.position.x, 0, unit2.transform.position.z);
        if(Vector3.SqrMagnitude(dir) < neighbourSquaredDistance)
        {
            //Check for navigation Line of Sight - this raycast can walk up slopes
            NavMeshHit hit;
            //If no hit, then within reach
            if(!NavMesh.Raycast(unit1.transform.position, unit2.transform.position, out hit, NavMesh.AllAreas))
            {
                return true;
            }
        }
        return false;
    }
}
