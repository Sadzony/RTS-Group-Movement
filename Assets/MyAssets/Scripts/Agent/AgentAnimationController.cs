using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentAnimationController : MonoBehaviour
{
    Animator animator;
    NavMeshAgent navMeshAgent;
    Vector2 velocity = Vector2.zero;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = transform.parent.GetComponent<NavMeshAgent>(); 
    }

    // Update is called once per frame
    void Update()
    {
        bool shouldMove = !navMeshAgent.isStopped && navMeshAgent.remainingDistance > navMeshAgent.radius;
        animator.SetBool("move", shouldMove);
    }
}
