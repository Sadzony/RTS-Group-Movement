using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

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

    //Maps a particular unit reference to a queue of commands
    protected Dictionary<Unit, Queue<Command>> unitQueueMap = new Dictionary<Unit, Queue<Command>>();

    //Keeps track of the units performing a command
    private Dictionary<Command, UnitList> commandMap = new Dictionary<Command, UnitList>();

    //Log keeping track of most recent commands
    private LinkedList<UnitList> log = new LinkedList<UnitList>();

    //Serialized copy of the log used for debug
    [SerializeField] private List<UnitList> _log = new List<UnitList>();

    //Methods

    //Unit created
    public void AddUnit(Unit unit)
    {
        if (!unitQueueMap.ContainsKey(unit))
            unitQueueMap.Add(unit, new Queue<Command>());
    }

    //Unit dies
    public void RemoveUnit(Unit unit) 
    {
        if(unitQueueMap.ContainsKey(unit))
        {
            CancelCommands(unit);

            unitQueueMap.Remove(unit);
        }
            
    }

    public Queue<Command> GetCommandQueue(Unit unit)
    {
        if (unitQueueMap.TryGetValue(unit, out Queue<Command> commandQueue))
        {
            return new Queue<Command>(commandQueue);
        }
        return null;
    }

#nullable enable
    //Returns true and a command if one exists, otherwise null and false
    public bool GetCurrentCommand(Unit unit, out Command? output)
    {
        if(unitQueueMap.TryGetValue(unit, out Queue<Command> commandQueue))
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
    //Checks if there's a command available after this one
    public bool NextCommandAvailable(Unit unit)
    {
        if (unitQueueMap.TryGetValue(unit, out Queue<Command> commandQueue))
        {
            if(commandQueue.Count > 1 && commandQueue.Peek().completionMap.TryGetValue(unit, out bool val) && val == true)
            {
                return true;
            }
        }
        return false;
    }
    public void OnCommandReceived(Unit unit)
    {
        if (unitQueueMap.TryGetValue(unit, out Queue<Command> commandQueue))
        {
            //If the top command is completed, pop it
            if (commandQueue.Peek().completionMap.TryGetValue(unit, out bool val) && val == true)
            {
                Command removedCommand = commandQueue.Dequeue();
                RemoveUnitFromCommandMap(removedCommand, unit);
            }
            SquadManager.Instance.AddUnitToSquad(commandQueue.Peek(), unit);
        }
    }
    public void OnCommandCompleted(Unit unit)
    {
        //Flags the command at the top of the queue as completed, making the unit pop it when it receives a new one
        if (unitQueueMap.TryGetValue(unit, out Queue<Command> commandQueue) && commandQueue.Count > 0)
        {
            Command command = commandQueue.Peek();
            SquadManager.Instance.RemoveUnitFromSquad(command, unit);

            //Flag the command as completed for that unit
            if (command.completionMap.TryGetValue(unit, out bool val))
                command.completionMap[unit] = true;
        }

    }
#nullable disable
    public void CommandUnit(Unit unit, Command command)
    {
        if(unitQueueMap.ContainsKey(unit))
        {
            //Cancel the unit's current commands
            if (unitQueueMap[unit].Count != 0) 
            {
                CancelCommands(unit);
                unitQueueMap[unit].Clear();
            }
            unitQueueMap[unit].Enqueue(command);

            //Start the command
            unit.ReceiveCommand();
        }
    }
    public void ShiftCommandUnit(Unit unit, Command command)
    {
        if(unitQueueMap.ContainsKey(unit))
        {
            unitQueueMap[unit].Enqueue(command);
        }
        //If unit is awaiting an order, start the command immediately
        if (unit.standby)
            unit.ReceiveCommand();
    }

    //Unit gets an override of commands or dies
    public void CancelCommands(Unit unit)
    {
        if (unitQueueMap.ContainsKey(unit))
        {
            unit.OnCommandCancel();
            //Update squad manager for each cancelled command
            foreach (Command queuedCommand in unitQueueMap[unit])
            {
                SquadManager.Instance.RemoveUnitFromSquad(queuedCommand, unit);
                RemoveUnitFromCommandMap(queuedCommand, unit);
            }
        }
        
    }

    public void UpdateCommandMap(Command command, HashSet<Unit> units)
    {
        //Make a dictionary entry at command
        if(!commandMap.ContainsKey(command))
        {
            UnitList unitSet = new UnitList(units);
            commandMap.Add(command, unitSet);

            //check if members match a previous entry in linked list
            bool logMatch = false;
            foreach (UnitList members in log)
            {
                HashSet<Unit> loggedMembers = members.GetMembers();
                if (units.SetEquals(loggedMembers))
                {
                    //If it does, bring the set to the top of the log and remove the previous log entry
                    log.Remove(members);
                    log.AddFirst(unitSet);
                    logMatch = true;
                    break;
                }
            }
            //If the new entry did not match the log, then log gets a new entry
            if (!logMatch)
            {
                log.AddFirst(unitSet);
            }
            OnValidate();

        }
    }
    public void RemoveUnitFromCommandMap(Command command, Unit unit)
    {
        //Remove unit from command map. If empty, remove the entry and the associated squad
        if(commandMap.TryGetValue(command, out UnitList unitSet))
        {
            unitSet.Remove(unit);
            //check the number of units still performing the command
            if (commandMap[command].Count() <= 0)
            {
                log.Remove(unitSet);
                commandMap.Remove(command);
                SquadManager.Instance.RemoveSquad(command);
            }
            OnValidate();
        }
    }
    public void RemoveFromCommandMap(Command command)
    {
        if (commandMap.TryGetValue(command, out UnitList unitSet))
        {
            log.Remove(unitSet);
            commandMap.Remove(command);
            SquadManager.Instance.RemoveSquad(command);
            OnValidate();
        }
    }
    public void OnValidate()
    {
        _log = new List<UnitList>(log);
    }

}