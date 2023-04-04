using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandManager : MonoBehaviour
{
    //Singleton Template
    private static CommandManager instance;
    public static CommandManager Instance
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

    //Maps a particular unit pointer to a queue of commands
    protected Dictionary<Unit, Queue<Command>> commandMap = new Dictionary<Unit, Queue<Command>>();

    //Methods
    public void AddUnit(Unit unit)
    {
        if (!commandMap.ContainsKey(unit))
            commandMap.Add(unit, new Queue<Command>());
    }
    public void RemoveUnit(Unit unit) 
    {
        if(commandMap.ContainsKey(unit))
        {
            //Cancel the commands and update squads
            foreach (Command queuedCommand in commandMap[unit])
            {
                SquadManager.Instance.RemoveUnitFromSquad(queuedCommand, unit);
            }
            commandMap.Remove(unit);
        }
            
    }
#nullable enable
    //Returns true and a command if one exists, otherwise null and false
    public bool GetCommand(Unit unit, out Command? output)
    {
        Queue<Command> commandQueue = new Queue<Command>();
        if(commandMap.TryGetValue(unit, out commandQueue))
        {
            if(commandQueue.Count != 0)
            {
                output = commandQueue.Peek();
                return true;
            }
        }
        output = null;
        return false;
    }
#nullable disable
    public void CommandUnit(Unit unit, Command command)
    {
        if(commandMap.ContainsKey(unit))
        {
            if (commandMap[unit].Count != 0) 
            {
                //Cancel the commands and update squads
                foreach(Command queuedCommand in commandMap[unit])
                {
                    SquadManager.Instance.RemoveUnitFromSquad(queuedCommand, unit);
                }

                commandMap[unit].Clear();
            }
            commandMap[unit].Enqueue(command);
        }
    }
    public void ShiftCommandUnit(Unit unit, Command command)
    {
        if(commandMap.ContainsKey(unit))
        {
            commandMap[unit].Enqueue(command);
        }
    }

}