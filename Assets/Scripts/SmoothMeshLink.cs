using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class SmoothMeshLink : MonoBehaviour
{
    public AnimationCurve linkCurve = new AnimationCurve();
    private NavMeshAgent navAgent;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        // make sure auto traverse mesh links are off
        navAgent.autoTraverseOffMeshLink = false;
        
        while (true) {
            if (navAgent.isOnOffMeshLink) {
                yield return StartCoroutine(SmoothCurve(0.5f));
            }
            yield return null;
        }
    }
    IEnumerator SmoothCurve(float dur) {
        OffMeshLinkData data = navAgent.currentOffMeshLinkData;
        Vector3 startPos = navAgent.transform.position;

        // end position has current vertical displacement added 
        Vector3 endPos = data.endPos + (navAgent.baseOffset * Vector3.up);

        float n_time = 0.0f;
        while (n_time < 1.0f) {
            float yOffset = linkCurve.Evaluate(n_time);
            navAgent.transform.position = Vector3.Lerp(startPos, endPos, n_time) + (yOffset * Vector3.up);
            if ((endPos - navAgent.transform.position).magnitude < 0.25f) {
                navAgent.CompleteOffMeshLink();
                yield break;
            }
            n_time += Time.deltaTime / dur;
            yield return null;
        }
    }
}
