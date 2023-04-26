using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Mouse_Detector : MonoBehaviour
{
    public bool tooltipEnabled = false;
    [SerializeField, TextArea]
    public string tooltipText;
    RectTransform rectTransform;
    void Start()
    {
        UIManager.Instance.detectors.Add(this);
        rectTransform = GetComponent<RectTransform>();
        
    }
    void OnDestroy()
    {
        UIManager.Instance.detectors.Remove(this);
    }
    public bool IsPointerOverObject()
    {
        Vector2 localMousePosition = rectTransform.InverseTransformPoint(Input.mousePosition);
        if (rectTransform.rect.Contains(localMousePosition))
        {
            return true;
        }
        return false;
    }
}
