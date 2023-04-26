using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandLineRenderer : MonoBehaviour
{
    Unit unitScript;
    LineRenderer line;
    public 

    // Start is called before the first frame update
    void Start()
    {
        unitScript = transform.GetComponent<Unit>();
        line = transform.Find("Command Path").GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!unitScript.isSelected)
        {
            line.gameObject.SetActive(false);
        }
        else
        {
            if(CommandManager.Instance.unitQueueMap.TryGetValue(unitScript, out Queue<Command> commandQueue) && commandQueue.Count > 0 && commandQueue.Peek().completionMap.TryGetValue(unitScript, out bool val) && val == false)
            {
                int index = 0;
                line.positionCount = commandQueue.Count;
                line.gameObject.SetActive(true);
                if (!UIManager.Instance.pathDebugEnabled)
                {
                    line.positionCount++;
                    line.SetPosition(index, transform.position + new Vector3(0, 0.5f, 0));
                    index++;
                }
                foreach (Command c in commandQueue)
                {
                    line.SetPosition(index, c.target + new Vector3(0, 0.5f, 0));
                    index++;
                }
            }
            else line.gameObject.SetActive(false);
        }
    }
}
