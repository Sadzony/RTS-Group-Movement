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
    [SerializeField] GameObject gameManager;
    public List<UI_Mouse_Detector> detectors;
    CameraMovement cameraMovementScript;
    Selector selectorSript;

    private void Start()
    {
        cameraMovementScript = Camera.main.transform.parent.Find("CameraFollowPoint").GetComponent<CameraMovement>();
        selectorSript = gameManager.GetComponent<Selector>();
    }
    private void Update()
    {
        bool mouseOverUI = false;
        foreach (UI_Mouse_Detector detector in detectors)
        {
            if (detector.mouseOverUI == true) mouseOverUI = true;
        }
        cameraMovementScript.mouseOverUI = mouseOverUI;
        selectorSript.mouseOverUI = mouseOverUI;


    }
}
