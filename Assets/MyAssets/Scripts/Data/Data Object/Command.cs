using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CommandType
{
    Empty,
    Movement,
    Interact
}
public class Command
{
    public Command(HashSet<Unit> members, Vector3 p_target)
    {
        //Initialize the completion list
        foreach(Unit unit in members)
        {
            completionMap.Add(unit, false);
        }
        target = p_target;
    }
    public Vector3 target;
    public CommandType type = CommandType.Empty;
    public Dictionary<Unit, bool> completionMap = new Dictionary<Unit, bool>();

}
