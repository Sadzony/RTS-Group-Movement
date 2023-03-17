using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{

    //Singleton Template
    private static SelectionManager instance;
    public static SelectionManager Instance 
    {
        get { return instance; } 
        private set { instance = value; }
    }
    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public HashSet<Unit> selectableUnits = new HashSet<Unit>();
    public HashSet<Unit> selectedUnits = new HashSet<Unit>();

    public void Select(Unit unit)
    {
        //Activate indicator and add to list
        unit.OnSelected();
        selectedUnits.Add(unit);
    }
    public void Deselect(Unit unit)
    {
        unit.OnDeselected();
        selectedUnits.Remove(unit);
    }
    public bool isSelected(Unit unit)
    {
        return selectedUnits.Contains(unit);
    }

    public void DeselectAll()
    {
        foreach(Unit unit in selectedUnits) 
        {
            unit.OnDeselected();
        }
        selectedUnits.Clear();
    }

    public void ClickSelect(Unit unit)
    {
        //Selects a single unit
        DeselectAll();
        Select(unit);
    }
    public void ShiftClickSelect(Unit unit) 
    {
        //If not in list, add to list, otherwise remove
        if(!isSelected(unit))
            Select(unit);
        else
            selectedUnits.Remove(unit);
    }
    public void ControlClickSelect(Unit unit) 
    {
        //Reverse of shift select
        if (isSelected(unit))
            Deselect(unit);
        else
            Select(unit);
    }
    public void DragSelect(Unit unit)
    {
        Select(unit);
    }
    //public void ShiftDragSelect(List<GameObject> units)
    //{
    //    foreach(GameObject unit in units)
    //    {
    //        if (!selectedUnits.Contains(unit))
    //            AddToSelected(unit);
    //    }
    //}
    //public void ControlDragSelected(List<GameObject> units)
    //{
    //    foreach(GameObject unit in units)
    //        Deselect(unit);
    //}
}
