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
    public Vector3 target;
    public CommandType type;
}
