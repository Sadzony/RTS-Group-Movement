using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

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
    public void SelectAll()
    {
        if(selectorScript.enabled == true)
        {
            List<Unit> selectable = SelectionManager.Instance.GetSelectable().ToList();
            foreach(Unit u in selectable)
            {
                SelectionManager.Instance.Select(u);
            }
        }
    }
    public void ToggleVelocityDebug(Button button)
    {
        UIManager.Instance.velocityDebugEnabled = !UIManager.Instance.velocityDebugEnabled;
        if(UIManager.Instance.velocityDebugEnabled)
        {
            button.image.color = new Color(0, 191, 255, 255);
            HashSet<Unit> selectable = SelectionManager.Instance.GetSelectable();
            foreach(Unit u in selectable)
            {
                u.GetComponent<DebugRenderer>().enabled = true;
            }
        }
        else
        {
            button.image.color = new Color(255, 255, 255, 255);
        }
    }
    public void ToggleNeighbourhoodDebug(Button button)
    {
        UIManager.Instance.neighbourhoodDebugEnabled = !UIManager.Instance.neighbourhoodDebugEnabled;
        if (UIManager.Instance.neighbourhoodDebugEnabled)
        {
            button.image.color = Color.green;
            HashSet<Unit> selectable = SelectionManager.Instance.GetSelectable();
            foreach (Unit u in selectable)
            {
                u.GetComponent<DebugRenderer>().enabled = true;
            }
        }
        else
        {
            button.image.color = new Color(255, 255, 255, 255);
        }
    }
    public void TogglePathDebug(Button button)
    {
        UIManager.Instance.pathDebugEnabled = !UIManager.Instance.pathDebugEnabled;
        if (UIManager.Instance.pathDebugEnabled)
        {
            button.image.color = Color.red;
            HashSet<Unit> selectable = SelectionManager.Instance.GetSelectable();
            foreach (Unit u in selectable)
            {
                u.GetComponent<DebugRenderer>().enabled = true;
            }
        }
        else
        {
            button.image.color = new Color(255, 255, 255, 255);
        }
    }
}
