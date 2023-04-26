using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    //Singleton Template
    private static UIManager instance;
    public static UIManager Instance
    {
        get { return instance; }
        private set { instance = value; }
    }
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }
    public List<UI_Mouse_Detector> detectors;
    bool mouseOverUI = false;
    public bool velocityDebugEnabled = false;
    public bool neighbourhoodDebugEnabled = false;
    public bool pathDebugEnabled = false;
    public bool getMouseOver()
    {
        return mouseOverUI;
    }
    private void Update()
    {
        mouseOverUI = false;
        bool tooltipEnabled = false;
        foreach (UI_Mouse_Detector detector in detectors)
        {
            if (detector.IsPointerOverObject())
            {
                mouseOverUI = true;
                if (detector.tooltipEnabled)
                {
                    tooltipEnabled = true;
                    Tooltip.Instance.Show(detector.tooltipText);
                }
            }
        }
        if (mouseOverUI && tooltipEnabled) {
            Tooltip.Instance.UpdateText();
            Tooltip.Instance.UpdateTooltipPosition();
        } 
        else Tooltip.Instance.Hide();
    }
}
