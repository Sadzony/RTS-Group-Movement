using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Mouse_Detector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector] public bool mouseOverUI;
    
    void Start()
    {
        UIManager.Instance.detectors.Add(this);
    }
    void OnDestroy()
    {
        UIManager.Instance.detectors.Remove(this);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseOverUI = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseOverUI = false;
    }
}
