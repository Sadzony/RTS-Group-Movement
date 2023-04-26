using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugRenderer : MonoBehaviour
{
    Unit unitScript;
    LineRenderer velocityDebug;
    LineRenderer neighbourhoodCircle;
    LineRenderer neighbourhoodConnections;
    LineRenderer pathDebug;
    LineRenderer goalDebug;

    [Min(10)] public int circleSteps;


    private void Start()
    {
        unitScript = transform.GetComponent<Unit>();
        velocityDebug = transform.Find("Debug Velocity").GetComponent<LineRenderer>();
        neighbourhoodCircle = transform.Find("Debug Neighbourhood").GetComponent<LineRenderer>();
        neighbourhoodConnections = neighbourhoodCircle.transform.Find("Connections").GetComponent<LineRenderer>();
        pathDebug = transform.Find("Debug Path").GetComponent<LineRenderer>();
        goalDebug = transform.Find("Debug Goal").GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (UIManager.Instance.velocityDebugEnabled || UIManager.Instance.neighbourhoodDebugEnabled || UIManager.Instance.pathDebugEnabled)
        {
            if (!unitScript.isSelected)
            {
                velocityDebug.gameObject.SetActive(false);
                neighbourhoodCircle.gameObject.SetActive(false);
                pathDebug.gameObject.SetActive(false);
                goalDebug.gameObject.SetActive(false);
            }
            else
            {
                if (UIManager.Instance.velocityDebugEnabled)
                {
                    velocityDebug.gameObject.SetActive(true);
                    velocityDebug.SetPosition(0, unitScript.transform.position + new Vector3(0, 0.5f, 0));
                    Vector3 velocityLine = unitScript.velocity.normalized * 2 * (unitScript.velocity.magnitude / SteeringData.Instance.unitSpeed);
                    velocityDebug.SetPosition(1, transform.position + velocityLine);
                }
                else
                    velocityDebug.gameObject.SetActive(false);
                if(UIManager.Instance.neighbourhoodDebugEnabled)
                {
                    //Set positions of circle to number of steps and radius of group neighbourhood.
                    neighbourhoodCircle.positionCount = circleSteps;
                    //Render circle
                    for(int i = 0; i < circleSteps; i++)
                    {
                        //The fraction of the circle that has been completed so far
                        float circumferenceFraction = (float)i / circleSteps;
                        
                        //Rotation of the point for the next step of the circle
                        float radian = circumferenceFraction * 2 * Mathf.PI;

                        float xPoint = Mathf.Cos(radian) * GroupManager.Instance.neighbourDistance;
                        float zPoint = Mathf.Sin(radian) * GroupManager.Instance.neighbourDistance;

                        Vector3 pointPosition = new Vector3(transform.position.x + xPoint, transform.position.y + 0.5f, transform.position.z + zPoint);
                        neighbourhoodCircle.SetPosition(i, pointPosition);
                    }
                    neighbourhoodCircle.gameObject.SetActive(true);

                    //Grab neighbours from neighbourmap of group and iterate. Set number of positions to count of neighbours * 3
                    #nullable enable
                    Group? currentGroup = GroupManager.Instance.GetGroup(unitScript);
                    #nullable disable
                    if(currentGroup != null && currentGroup.neighbourMap.TryGetValue(unitScript, out HashSet<Unit> neighbours) && neighbours != null && neighbours.Count > 0)
                    {
                        neighbourhoodConnections.positionCount = neighbours.Count * 3;
                        int connectionIndex = 0;
                        foreach(Unit neighbour in neighbours)
                        {
                            neighbourhoodConnections.SetPosition(connectionIndex, transform.position + new Vector3(0, 0.4f, 0));
                            connectionIndex++;
                            neighbourhoodConnections.SetPosition(connectionIndex, neighbour.transform.position + new Vector3(0, 0.5f, 0));
                            connectionIndex++;
                            neighbourhoodConnections.SetPosition(connectionIndex, transform.position + new Vector3(0, 0.4f, 0));
                            connectionIndex++;
                        }
                        neighbourhoodConnections.gameObject.SetActive(true);
                    }
                    else
                        neighbourhoodConnections.gameObject.SetActive(false);
                }
                else
                    neighbourhoodCircle.gameObject.SetActive(false);

                if(UIManager.Instance.pathDebugEnabled)
                {
                    #nullable enable
                    Group? currentGroup = GroupManager.Instance.GetGroup(unitScript);
                    #nullable disable
                    if (currentGroup != null)
                    {
                        goalDebug.gameObject.SetActive(true);
                        //Set positions of circle to number of steps and radius of group neighbourhood.
                        goalDebug.positionCount = circleSteps;
                        //Render circle
                        for (int i = 0; i < circleSteps; i++)
                        {
                            //The fraction of the circle that has been completed so far
                            float circumferenceFraction = (float)i / circleSteps;

                            //Rotation of the point for the next step of the circle
                            float radian = circumferenceFraction * 2 * Mathf.PI;

                            float xPoint = Mathf.Cos(radian) * currentGroup.finishDistance;
                            float zPoint = Mathf.Sin(radian) * currentGroup.finishDistance;

                            Vector3 pointPosition = new Vector3(currentGroup.goalCentre.x + xPoint, currentGroup.goalNode.position.y + 0.3f, currentGroup.goalCentre.z + zPoint);
                            goalDebug.SetPosition(i, pointPosition);
                        }

                        if (currentGroup.unitPaths.TryGetValue(unitScript, out Waypoint currentTarget) && currentTarget != null)
                        {
                            pathDebug.gameObject.SetActive(true);
                            Waypoint currentNode = currentTarget;
                            pathDebug.positionCount = 1;
                            pathDebug.SetPosition(0, transform.position + new Vector3(0, 0.3f, 0));
                            int lineIndex = 1;
                            while (currentNode != null)
                            {
                                pathDebug.positionCount = pathDebug.positionCount + 1;
                                pathDebug.SetPosition(lineIndex, currentNode.position + new Vector3(0, 0.3f, 0));
                                lineIndex++;

                                if (currentNode == currentGroup.goalNode)
                                {

                                }
                                currentNode = currentNode.next;
                            }
                        }
                        else
                            pathDebug.gameObject.SetActive(false);
                    }
                    else
                    {
                        pathDebug.gameObject.SetActive(false);
                        goalDebug.gameObject.SetActive(false);
                    }
                }
                else
                {
                    pathDebug.gameObject.SetActive(false);
                    goalDebug.gameObject.SetActive(false);
                }

            }
        }
        else
        {
            velocityDebug.gameObject.SetActive(false);
            neighbourhoodCircle.gameObject.SetActive(false);
            pathDebug.gameObject.SetActive(false);
            goalDebug.gameObject.SetActive(false);
            this.enabled = false;
        }
    }
}
