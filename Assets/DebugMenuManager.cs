using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DebugMenuManager : MonoBehaviour
{

    [SerializeField] GameObject gameManager;
    Selector selectorScript;
    UnitSpawner unitSpawnerScript;

    private void Start()
    {
        selectorScript = GetComponent<Selector>();
        unitSpawnerScript= GetComponent<UnitSpawner>();
    }

    public void DeleteSelected()
    {
        if (selectorScript.enabled == true)
        {
            selectorScript.enabled = false;
            List<Unit> selected = SelectionManager.Instance.GetSelected().ToList();
            foreach (Unit unit in selected)
            {
                unit.Die();
            }
            selectorScript.enabled = true;
        }
    }
    public void UnitSpawnMode()
    {
        SelectionManager.Instance.DeselectAll();
        selectorScript.enabled = false;
        unitSpawnerScript.enabled = true;
    }
    public void CursorMode()
    {
        unitSpawnerScript.enabled = false;
        selectorScript.enabled = true;
    }
    public void CompleteSelected()
    {
        if (selectorScript.enabled == true)
        {
            List<Unit> selected = SelectionManager.Instance.GetSelected().ToList();
            foreach(Unit unit in selected)
            {
                unit.CompleteCommand();
            }
        }
            
    }
}
