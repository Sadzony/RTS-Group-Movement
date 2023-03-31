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

    //List used to output the active squads to screen
    [SerializeField] protected List<Squad> activeSquads = new List<Squad>();

    //Dictionary mapping commands to squads
    protected Dictionary<Command, Squad> squads = new Dictionary<Command, Squad>();

    //Functions
    public void AddSquad(Command command, List<Unit> squadMembers)
    {
        Squad squad = new Squad();
        squad.SetSquad(squadMembers);
        if (!squads.ContainsKey(command))
        {
            squads.Add(command, squad);
            activeSquads.Insert(0, squad);
        }
    }
    public void RemoveUnitFromSquad(Command command, Unit unit) 
    {
        Squad foundSquad;
        if(squads.TryGetValue(command, out foundSquad))
        {
            foundSquad.RemoveUnit(unit);
            //If the squad has become empty, remove it from dictionary
            if(foundSquad.Count() <= 0)
            {
                squads.Remove(command);
                activeSquads.Remove(foundSquad);
            }
        }
    }
}
