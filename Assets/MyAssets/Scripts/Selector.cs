using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Selector : MonoBehaviour
{ 

    [SerializeField] RectTransform selectionBox;
    [SerializeField] LayerMask SelectCastLayer;

    Vector3 selectionStartPos;
    Vector3 selectionEndPos;
    bool selectionStarted;
    bool boxSelection;
    Unit clickedUnit;

    [HideInInspector] public bool mouseOverUI;
    private void Start()
    {
        selectionStartPos = Vector3.zero;
        selectionEndPos = Vector3.zero;
        boxSelection = false;
        selectionStarted = true;
        clickedUnit = null;
    }
    private void Update()
    {
        //On Click
        if (Input.GetMouseButtonDown(0) && !mouseOverUI) 
        {
            clickedUnit = null;
            //Send a raycast from mouse position and select that unit
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, SelectCastLayer))
            {
                SelectionManager.Instance.selectableUnits.TryGetValue(hit.transform.parent.gameObject.GetComponent<Unit>(), out clickedUnit);
                if(SelectionManager.Instance.selectableUnits.Contains(clickedUnit))
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    SelectionManager.Instance.ShiftClickSelect(clickedUnit);

                }
                else if (Input.GetKey(KeyCode.LeftControl))
                {
                    SelectionManager.Instance.ControlClickSelect(clickedUnit);
                }
                else
                {
                    SelectionManager.Instance.ClickSelect(clickedUnit);
                }
            }
            else 
            {
                if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.LeftControl))
                    SelectionManager.Instance.DeselectAll();
            }
            selectionStarted = true;
            //Set start position for selection box
            selectionStartPos = Input.mousePosition;
        }
        //On Hold
        if(Input.GetMouseButton(0) && selectionStarted)
        {
            //Update the end selection position
            selectionEndPos = Input.mousePosition;
            Vector3 startToEnd = selectionEndPos - selectionStartPos;

            ////Find if the user moved the mouse from start pos
            if (startToEnd.sqrMagnitude > 0.1f)
            {
                boxSelection = true;
                selectionBox.gameObject.SetActive(true);
                Rect selectionRect = new Rect();
                ResizeSelectionBox(startToEnd, ref selectionRect);
                foreach (Unit unit in SelectionManager.Instance.selectableUnits)
                {
                    if (selectionRect.Contains(Camera.main.WorldToScreenPoint(unit.gameObject.transform.position)))
                    {
                        if (Input.GetKey(KeyCode.LeftControl))
                            SelectionManager.Instance.Deselect(unit);
                        else
                            SelectionManager.Instance.Select(unit);
                    }
                    else if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.LeftControl) && unit != clickedUnit)
                        SelectionManager.Instance.Deselect(unit);
                }

            }
            else
            {
                boxSelection = false;
                selectionBox.sizeDelta = Vector3.zero;
                selectionBox.gameObject.SetActive(false);
            }
        }

        //On release
        if(Input.GetMouseButtonUp(0))
        {
            if(boxSelection)
            {
                boxSelection = false;
                selectionBox.sizeDelta = Vector3.zero;
                selectionBox.gameObject.SetActive(false);
                clickedUnit = null;
            }
            selectionStarted = false;
        }
    }

    void ResizeSelectionBox(Vector2 startToEnd, ref Rect selectionRect) 
    {
        //Set the box position to start position
        selectionBox.anchoredPosition = new Vector2(selectionStartPos.x, selectionStartPos.y);
        //Scale the box to match the start and end position
        selectionBox.sizeDelta = new Vector2(Mathf.Abs(startToEnd.x), Mathf.Abs(startToEnd.y));

        //Flip the box:

        //Left
        if (startToEnd.x < 0)
        {
            selectionBox.localScale = new Vector3(-1, selectionBox.localScale.y, selectionBox.localScale.z);
            selectionRect.xMin = selectionEndPos.x;
            selectionRect.xMax = selectionStartPos.x;

        }
        //Right
        else
        {
            selectionBox.localScale = new Vector3(1, selectionBox.localScale.y, selectionBox.localScale.z);
            selectionRect.xMin = selectionStartPos.x;
            selectionRect.xMax = selectionEndPos.x;
        }
        //Down
        if (startToEnd.y < 0)
        {
            selectionBox.localScale = new Vector3(selectionBox.localScale.x, -1, selectionBox.localScale.z);
            selectionRect.yMin = selectionEndPos.y;
            selectionRect.yMax = selectionStartPos.y;
        }
        //Up
        else
        {
            selectionBox.localScale = new Vector3(selectionBox.localScale.x, 1, selectionBox.localScale.z);
            selectionRect.yMin = selectionStartPos.y;
            selectionRect.yMax = selectionEndPos.y;
        }
    }
    private void OnDisable()
    {
        boxSelection = false;
        if (selectionBox != null)
        {
            selectionBox.sizeDelta = Vector3.zero;
            selectionBox.gameObject.SetActive(false);
        }
        selectionStartPos = Vector3.zero;
        selectionEndPos = Vector3.zero;
        clickedUnit = null;
        selectionStarted = false;
    }
}
