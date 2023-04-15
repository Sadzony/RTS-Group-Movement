using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

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
            //If location is valid, check selection and make command
            if (Physics.Raycast(ray, out hit))
            {
                HashSet<Unit> selectedUnits = SelectionManager.Instance.GetSelected();

                if (selectedUnits.Count > 0)
                {
                    //Find the target point on the navmesh
                    NavMeshHit aiHit;
                    if (NavMesh.SamplePosition(hit.point, out aiHit, Mathf.Infinity, NavMesh.AllAreas))
                    {
                        Command command = new Command(selectedUnits, aiHit.position);
                        command.type = CommandType.Movement;

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
}
