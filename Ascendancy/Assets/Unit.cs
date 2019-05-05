using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    NavMeshAgent navMeshAgent;

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        Vector3 target = new Vector3(10, 0, 10);
        navMeshAgent.SetDestination(target);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
