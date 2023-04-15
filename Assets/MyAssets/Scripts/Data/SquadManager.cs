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

    //Debug List used to output the active squads to screen
    private HashSet<Squad> activeSquads = new HashSet<Squad>();
    [SerializeField] private List<Squad> _activeSquads = new List<Squad>();

    //Dictionary mapping commands to squads
    private Dictionary<Command, Squad> squads = new Dictionary<Command, Squad>();

    //Functions
    public void AddSquad(Command command)
    {
        Squad squad = new Squad(command);
        if(!squads.ContainsKey(command))
        {
            squads.Add(command, squad);
            GroupManager.Instance.AddSquadronEntry(squad);
            activeSquads.Add(squad);
            OnValidate();
        }
    }

    //A command is cancelled or emptied
    public void RemoveSquad(Command command)
    {
        if (squads.TryGetValue(command, out Squad squad))
        {
            squad.Clear();
            activeSquads.Remove(squad);
            GroupManager.Instance.RemoveSquadronEntry(squad);
            squads.Remove(command);
            OnValidate();
        }
    }

    //Unit starts a command
    public void AddUnitToSquad(Command command, Unit unit)
    {
        if (squads.TryGetValue(command, out Squad squad))
        {
            squad.Add(unit);
            OnValidate();
            GroupManager.Instance.ManageUnit(unit);

        }
    }
    //Unit replaces an old command with a new one
    public void RemoveUnitFromSquad(Command command, Unit unit) 
    {
        //Remove unit from squad
        if (squads.TryGetValue(command, out Squad squad))
        {
            squad.Remove(unit);
            OnValidate();
            GroupManager.Instance.UnmanageUnit(unit);
        }
    }
    private void OnValidate()
    {
        _activeSquads = new List<Squad>(activeSquads);
    }
}