using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CommandMaker : MonoBehaviour
{
    void Update()
    {
        //On right click
        if(Input.GetMouseButtonDown(1) && !UIManager.Instance.getMouseOver())
        {
            //Find the clicked location
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                //If location is valid, make a command with it
                Command command = new Command();
                command.target = hit.point;
                command.type = CommandType.Movement;

                HashSet<Unit> selectedUnits = SelectionManager.Instance.GetSelected();

                if (selectedUnits.Count > 0)
                {
                    SquadManager.Instance.AddSquad(command);
                    CommandManager.Instance.UpdateCommandMap(command, selectedUnits);

                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        //Get selected units
                        foreach (Unit unit in selectedUnits)
                        {
                            //Add command to queue
                            CommandManager.Instance.ShiftCommandUnit(unit, command);
                        }
                    }
                    else
                    {
                        //Get selected units
                        foreach (Unit unit in selectedUnits)
                        {
                            //Override commands
                            CommandManager.Instance.CommandUnit(unit, command);
                        }
                    }
                }
            }
        }


    }
}
