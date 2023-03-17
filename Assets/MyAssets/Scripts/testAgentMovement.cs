using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class testAgentMovement : MonoBehaviour
{
    NavMeshAgent myNavMeshAgent;
    Unit agentLogic;
    void Start()
    {
        myNavMeshAgent = GetComponent<NavMeshAgent>();
        agentLogic = GetComponent<Unit>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            SetDestinationToMousePosition();
        }
    }

    void SetDestinationToMousePosition()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            if (agentLogic.isSelected)
                myNavMeshAgent.SetDestination(hit.point);

        }
    }
}
