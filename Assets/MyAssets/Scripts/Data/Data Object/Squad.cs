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
    ~UnitList()
    {
        list.Clear();
        set.Clear();
    }
    public int Count()
    {
        return list.Count;
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
        if (set.Add(unit))
            list.Add(unit);
    }
    public bool Remove(Unit unit)
    {
        if (set.Remove(unit))
        {
            list.Remove(unit);
            return true;
        }
        return false;
    }
}

[System.Serializable]
public class Squad : UnitList
{
    private Command command;

    public Squad() : base()
    {
        command = new Command();
    }
    public Squad(Squad copy) : base(copy)
    {
        copy.command = command;
    }
}
