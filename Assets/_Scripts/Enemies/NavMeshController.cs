using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshController : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform target;
    
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Space))
            agent.destination = target.position;
    }
}
