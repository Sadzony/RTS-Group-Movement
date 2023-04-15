using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentAnimationController : MonoBehaviour
{
    Animator animator;
    NavMeshAgent unitAgent;
    Unit unitScript;
    Vector2 velocity = Vector2.zero;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        unitScript = transform.parent.GetComponent<Unit>();
        unitAgent = transform.parent.GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        //bool shouldMove = !unitAgent.isStopped && unitAgent.remainingDistance > unitAgent.radius;
        bool shouldMove = unitScript.isMoving;
        animator.SetBool("move", shouldMove);
    }
}
