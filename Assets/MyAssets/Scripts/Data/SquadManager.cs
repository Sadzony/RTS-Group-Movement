using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadManager : MonoBehaviour
{
    //Singleton Template
    private static SquadManager instance;
    public static SquadManager Instance
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

    //Serializable log used for squad selection mechanic
    [SerializeField] protected List<UnitList> log = new List<UnitList>();

    protected int logSize = 5;

    //List used to output the active squads to screen
    [SerializeField] protected List<Squad> activeSquads = new List<Squad>();

    //Dictionary mapping commands to squads
    protected Dictionary<Command, Squad> squads = new Dictionary<Command, Squad>();

    //Functions
    public void AddSquad(Command command, HashSet<Unit> squadMembers)
    {
        Squad squad = new Squad();
        squad.SetMembers(squadMembers);

        //Create dictionary entry
        if (!squads.ContainsKey(command))
        {
            squads.Add(command, squad);
            //activeSquads.Insert(0, squad);

            //Check if the members match a recent log entry
            bool logMatch = false;
            foreach (UnitList units in log)
            {
                HashSet<Unit> loggedMembers = units.GetMembers();
                if (squadMembers.SetEquals(loggedMembers))
                {
                    //If it does, bring that set to the top of the list
                    log.Remove(units);
                    log.Insert(0, units);
                    logMatch = true;
                    break;
                }
            }
            //If the new entry did not match the log, then log gets a new entry
            if (!logMatch)
            {
                //If log isn't full, add list at beginning
                if (log.Count < logSize)
                    log.Insert(0, new UnitList(squad));
                else
                {
                    //otherwise make space by removing last entry
                    log.RemoveAt(log.Count - 1);
                    log.Insert(0, new UnitList(squad));
                }
            }
        }
    }
    public void RemoveUnitFromSquad(Command command, Unit unit, bool removeFromLog = true) 
    {
        Squad foundSquad;
        if(squads.TryGetValue(command, out foundSquad))
        {
            foundSquad.Remove(unit);
            if(removeFromLog)
            {
                foreach(UnitList units in log)
                {
                    //Removes the unit from the log, if it is a member
                    if (units.Remove(unit))
                    {
                        if (units.Count() <= 0)
                            log.Remove(units);
                        break;
                    }
                }
            }
            //If the squad has become empty, remove it from dictionary
            if(foundSquad.Count() <= 0)
            {
                squads.Remove(command);
                //activeSquads.Remove(foundSquad);
            }
        }
    }
}
