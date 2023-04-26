using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SquadSelector : MonoBehaviour
{
    //Singleton Template
    private static SquadSelector instance;
    public static SquadSelector Instance
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
    List<HashSet<Unit>> selectionSquads = new List<HashSet<Unit>>();
    public Text text1;
    public Text text2;
    public Text text3;
    public Text text4;
    public Text text5;

    public CameraMovement movement;

    bool tap = false;
    string lastButton = "";
    public float doubleTapTime = 0.15f;
    float tapElapsedTime = 0;
    public void UpdateList(LinkedList<UnitList> log)
    {
        List<UnitList> logAsList = new List<UnitList>(log);
        selectionSquads.Clear();
        for(int i = 0; i < logAsList.Count; i++)
        {
            selectionSquads.Add(logAsList[i].set);
        }
    }
    private void Update()
    {
        if(tap)
        {
            tapElapsedTime += Time.deltaTime;
            if(tapElapsedTime > doubleTapTime)
            {
                tapElapsedTime = 0;
                tap = false;
                lastButton = "";
            }
        }
        for(int i = 0; i < selectionSquads.Count; i++) 
        { 
            if(i == 0)
            {
                text1.gameObject.SetActive(true);
                text1.text = selectionSquads[i].Count.ToString();
            }
            else if (i == 1)
            {
                text2.gameObject.SetActive(true);
                text2.text = selectionSquads[i].Count.ToString();
            }
            else if(i == 2)
            {
                text3.gameObject.SetActive(true);
                text3.text = selectionSquads[i].Count.ToString();
            }
            else if(i == 3)
            {
                text4.gameObject.SetActive(true);
                text4.text = selectionSquads[i].Count.ToString();
            }
            else if(i == 4)
            {
                text5.gameObject.SetActive(true);
                text5.text = selectionSquads[i].Count.ToString();
            }
        }
        if (selectionSquads.Count < 5)
        {
            for(int j = selectionSquads.Count; j < 5; j++)
            {
                if (j == 0)
                {
                    text1.gameObject.SetActive(false);
                }
                else if (j == 1)
                {
                    text2.gameObject.SetActive(false);
                }
                else if (j == 2)
                {
                    text3.gameObject.SetActive(false);
                }
                else if (j == 3)
                {
                    text4.gameObject.SetActive(false);
                }
                else if (j == 4)
                {
                    text5.gameObject.SetActive(false);
                }
            }
        }
        
        //Input
        if(Input.GetKeyDown("1") && selectionSquads.Count > 0)
        {
            if (!tap || lastButton != "1")
            {
                tap = true;
                tapElapsedTime = 0;
                lastButton = "1";
                if (!Input.GetKey(KeyCode.LeftShift))
                    SelectionManager.Instance.DeselectAll();
                foreach (Unit u in selectionSquads[0])
                {
                    SelectionManager.Instance.Select(u);
                }
            }
            //Double tap
            else if (tap && lastButton == "1")
            {
                tap = false;
                tapElapsedTime = 0;
                lastButton = "";
                Vector3 averagePosition = Vector3.zero;
                foreach (Unit u in selectionSquads[0])
                {
                    averagePosition += new Vector3(u.gameObject.transform.position.x, 35, u.gameObject.transform.position.z);
                }
                averagePosition /= selectionSquads[0].Count;
                averagePosition += new Vector3(0, 0, -20);
                movement.transform.position = averagePosition;
            }
        }
        else if (Input.GetKeyDown("2") && selectionSquads.Count > 1)
        {
            if (!tap || lastButton != "2")
            {
                tap = true;
                tapElapsedTime = 0;
                lastButton = "2";
                if (!Input.GetKey(KeyCode.LeftShift))
                    SelectionManager.Instance.DeselectAll();
                foreach (Unit u in selectionSquads[1])
                {
                    SelectionManager.Instance.Select(u);
                }
            }
            //Double tap
            else if (tap && lastButton == "2")
            {
                tap = false;
                tapElapsedTime = 0;
                lastButton = "";
                Vector3 averagePosition = Vector3.zero;
                foreach (Unit u in selectionSquads[1])
                {
                    averagePosition += new Vector3(u.gameObject.transform.position.x, 35, u.gameObject.transform.position.z);
                }
                averagePosition /= selectionSquads[1].Count;
                averagePosition += new Vector3(0, 0, -20);
                movement.transform.position = averagePosition;
            }
        }
        else if (Input.GetKeyDown("3") && selectionSquads.Count > 2)
        {
            if (!tap || lastButton != "3")
            {
                tapElapsedTime = 0;
                tap = true;
                lastButton = "3";
                if (!Input.GetKey(KeyCode.LeftShift))
                    SelectionManager.Instance.DeselectAll();
                foreach (Unit u in selectionSquads[2])
                {
                    SelectionManager.Instance.Select(u);
                }
            }
            //Double tap
            else if (tap && lastButton == "3")
            {
                tap = false;
                tapElapsedTime = 0;
                lastButton = "";
                Vector3 averagePosition = Vector3.zero;
                foreach (Unit u in selectionSquads[2])
                {
                    averagePosition += new Vector3(u.gameObject.transform.position.x, 35, u.gameObject.transform.position.z);
                }
                averagePosition /= selectionSquads[2].Count;
                averagePosition += new Vector3(0, 0, -20);
                movement.transform.position = averagePosition;
            }
        }
        else if (Input.GetKeyDown("4") && selectionSquads.Count > 3)
        {
            if (!tap || lastButton != "4")
            {
                tapElapsedTime = 0;
                tap = true;
                lastButton = "4";
                if (!Input.GetKey(KeyCode.LeftShift))
                    SelectionManager.Instance.DeselectAll();
                foreach (Unit u in selectionSquads[3])
                {
                    SelectionManager.Instance.Select(u);
                }
            }
            //Double tap
            else if (tap && lastButton == "4")
            {
                tap = false;
                tapElapsedTime = 0;
                lastButton = "";
                Vector3 averagePosition = Vector3.zero;
                foreach (Unit u in selectionSquads[3])
                {
                    averagePosition += new Vector3(u.gameObject.transform.position.x, 35, u.gameObject.transform.position.z);
                }
                averagePosition /= selectionSquads[3].Count;
                averagePosition += new Vector3(0, 0, -20);
                movement.transform.position = averagePosition;
            }
        }
        else if (Input.GetKeyDown("5") && selectionSquads.Count > 4)
        {
            if (!tap || lastButton != "5")
            {
                tapElapsedTime = 0;
                tap = true;
                lastButton = "5";
                if (!Input.GetKey(KeyCode.LeftShift))
                    SelectionManager.Instance.DeselectAll();
                foreach (Unit u in selectionSquads[4])
                {
                    SelectionManager.Instance.Select(u);
                }
            }
            //Double tap
            else if(tap && lastButton == "5")
            {
                tap = false;
                tapElapsedTime = 0;
                lastButton = "";
                Vector3 averagePosition = Vector3.zero;
                foreach (Unit u in selectionSquads[4])
                {
                    averagePosition += new Vector3(u.gameObject.transform.position.x, 0, u.gameObject.transform.position.z);
                }
                averagePosition /= selectionSquads[4].Count;
                averagePosition += new Vector3(0, 0, -20);
                movement.transform.position = averagePosition;
            }
        }
    }
}
