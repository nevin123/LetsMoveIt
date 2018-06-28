using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SetDestination : MonoBehaviour {

    NavMeshAgent agent;
    public Transform newPos;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(newPos.position);
    }
}
