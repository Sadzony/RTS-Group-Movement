using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class CommandMaker : MonoBehaviour
{
    public GameObject marker;
    public int circleSteps;
    public float markerRadius;
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

                        //Make the marker
                        command.marker = Instantiate(marker, aiHit.position, Quaternion.identity).GetComponent<LineRenderer>();
                        command.marker.positionCount = circleSteps;
                        for (int i = 0; i < circleSteps; i++)
                        {
                            //The fraction of the circle that has been completed so far
                            float circumferenceFraction = (float)i / circleSteps;

                            //Rotation of the point for the next step of the circle
                            float radian = circumferenceFraction * 2 * Mathf.PI;

                            float xPoint = Mathf.Cos(radian) * markerRadius;
                            float zPoint = Mathf.Sin(radian) * markerRadius;

                            Vector3 pointPosition = new Vector3(aiHit.position.x + xPoint, aiHit.position.y + 0.7f, aiHit.position.z + zPoint);
                            command.marker.SetPosition(i, pointPosition);
                        }

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
