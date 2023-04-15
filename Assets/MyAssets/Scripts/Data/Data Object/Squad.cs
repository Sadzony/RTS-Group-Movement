using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class UnitList
{
    [SerializeField] private List<Unit> list;
    private HashSet<Unit> set;

    public UnitList()
    {
        list = new List<Unit>();
        set = new HashSet<Unit>();
    }
    public UnitList(UnitList copy) 
    {
        list = new List<Unit>(copy.list);
        set = new HashSet<Unit>(copy.set);
    }
    public UnitList(HashSet<Unit> p_set)
    {
        set = new HashSet<Unit>(p_set);
        list = new List<Unit>(set);
    }
    ~UnitList()
    {
        Clear();
    }
    public virtual void Clear()
    {
        list.Clear();
        set.Clear();
    }
    public int Count()
    {
        return set.Count;
    }
    public bool isMember(Unit unit)
    {
        return set.Contains(unit);
    }
    public void SetMembers(HashSet<Unit> members)
    {
        list.Clear();
        set.Clear();
        set = new HashSet<Unit>(members);
        list = new List<Unit>(members);
    }
    public HashSet<Unit> GetMembers() { return new HashSet<Unit>(set); }
    public void Add(Unit unit)
    {
        list.Clear();
        set.Add(unit);
        list = new List<Unit>(set);
    }
    public bool Remove(Unit unit)
    {
        list.Clear();
        if (set.Remove(unit))
        {
            list = new List<Unit>(set);
            return true;
        }
        list = new List<Unit>(set);
        return false;
    }
}

[System.Serializable]
public class Squad : UnitList
{
    public Command command;
    public Waypoint goalNode;
    public override void Clear()
    {
        base.Clear();
    }
    public Squad(Squad copy) : base(copy)
    {
        copy.command = command;
        if (copy.goalNode != null)
            goalNode = copy.goalNode;
        else
            goalNode = new Waypoint(command.target);
    }
    public Squad(Command p_command) : base()
    {
        command = p_command;
        goalNode = new Waypoint(command.target);
    }
    public Squad(HashSet<Unit> p_set, Command p_command) : base(p_set)
    {
        command = p_command;
        goalNode = new Waypoint(command.target);
    }
}
