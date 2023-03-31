using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Squad
{
    [SerializeField] protected List<Unit> _squadMembers = new List<Unit>();

    protected HashSet<Unit> squadMembers = new HashSet<Unit>();

    public int Count()
    {
        return squadMembers.Count;
    }
    public bool isPartOfSquad(Unit unit)
    {
        return squadMembers.Contains(unit);
    }

    public void SetSquad(List<Unit> members)
    {
        squadMembers.Clear();
        _squadMembers.Clear();
        foreach (Unit unit in members)
        {
            squadMembers.Add(unit);
            _squadMembers.Add(unit);
        }
    }
    public void AddUnit(Unit unit)
    {
        if (squadMembers.Add(unit))
            _squadMembers.Add(unit);
    }
    public void RemoveUnit(Unit unit)
    {
        if(squadMembers.Remove(unit))
            _squadMembers.Remove(unit);
    }
}
