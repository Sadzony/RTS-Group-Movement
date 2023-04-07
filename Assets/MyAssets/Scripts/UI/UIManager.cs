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

    public bool getMouseOver()
    {
        bool mouseOverUI = false;
        foreach (UI_Mouse_Detector detector in detectors)
        {
            if (detector.mouseOverUI == true) mouseOverUI = true;
        }
        return mouseOverUI;


    }
}
