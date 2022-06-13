using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshDemo : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        // navMeshAgent.SetDestination(player.transform.position);
    }
    public void MoveAgent(Vector3 dest) {
        navMeshAgent.SetDestination(dest);
    }
}
