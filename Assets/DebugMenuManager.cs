using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DebugMenuManager : MonoBehaviour
{
    [SerializeField] GameObject unitPrefab;
    [SerializeField] GameObject gameManager;
    Selector selectorScript;
    private void Start()
    {
        selectorScript = gameManager.GetComponent<Selector>();
    }

    public void DeleteSelected()
    {
        selectorScript.enabled = false;
        foreach(Unit unit in SelectionManager.Instance.selectedUnits.ToList())
        {
            unit.Die();
        }
        selectorScript.enabled = true;
    }
    public void InstantiateUnit()
    {
        GameObject.Instantiate(unitPrefab, new Vector3(0, 0, 0), Quaternion.Euler(0,0,0));
    }
}
