using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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


    //Log used for squad selection mechanic
    private LinkedList<Squad> log = new LinkedList<Squad>();
    //Serialized copy of the log
    [SerializeField] List<Squad> _log = new List<Squad>();

    //Debug List used to output the active squads to screen
    private HashSet<Squad> activeSquads = new HashSet<Squad>();
    //Serialized copy of the above list
    [SerializeField] List<Squad> _activeSquads = new List<Squad>();

    //Dictionary mapping commands to squads
    private Dictionary<Command, Squad> squads = new Dictionary<Command, Squad>();

    //Functions
    public void AddSquad(Command command, HashSet<Unit> squadMembers)
    {
        Squad squad = new Squad(squadMembers, command);

        //Create dictionary entry
        if (!squads.ContainsKey(command))
        {
            squads.Add(command, squad);
            activeSquads.Add(squad);

            //Check if the squads' in the log unit hash set matches the newly added squad's
            bool logMatch = false;
            foreach (Squad members in log)
            {
                HashSet<Unit> loggedMembers = members.GetMembers();
                if (squadMembers.SetEquals(loggedMembers))
                {
                    //If it does, bring the new squad to the top of the log and remove the previous log entry
                    log.Remove(members);
                    log.AddFirst(squad);
                    logMatch = true;
                    break;
                }
            }
            //If the new entry did not match the log, then log gets a new entry
            if (!logMatch)
            {
                log.AddFirst(squad);
            }
        }
        //Update serialized properties
        _log = new List<Squad>(log);
        _activeSquads = new List<Squad>(activeSquads);
    }
    public void RemoveUnitFromSquad(Command command, Unit unit) 
    {
        Squad foundSquad;
        if(squads.TryGetValue(command, out foundSquad))
        {
            foundSquad.Remove(unit);
            //If the squad has become empty, remove it from dictionary
            if(foundSquad.Count() <= 0)
            {
                squads.Remove(command);
                activeSquads.Remove(foundSquad);
                log.Remove(foundSquad);
            }
        }
        //Update serialized properties
        _log = new List<Squad>(log);
        _activeSquads = new List<Squad>(activeSquads);
    }
}