using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class SmoothMeshLink : MonoBehaviour
{
    private AnimationCurve linkCurve = new AnimationCurve();
    private NavMeshAgent navAgent;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        // make sure auto traverse mesh links are off
        navAgent.autoTraverseOffMeshLink = false;

        while (true) {
            if (navAgent.isOnOffMeshLink) {
                yield return StartCoroutine(SmoothCurve());
            }
        }
    }

    IEnumerator SmoothCurve() {
        yield return null;
    }
}
