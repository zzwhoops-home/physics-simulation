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
                yield return StartCoroutine(SmoothCurve());
            }
            yield return null;
        }
    }
    IEnumerator SmoothCurve() {
        OffMeshLinkData data = navAgent.currentOffMeshLinkData;
        Vector3 startPos = navAgent.transform.position;

        // end position has current vertical displacement added 
        Vector3 endPos = data.endPos + (navAgent.baseOffset * Vector3.up);

        Vector3 distance = endPos - startPos;
        float horDistance = (new Vector2(distance.x, distance.z).magnitude);
        float vertDistance = distance.y;

        // simulate jumping, kind of
        float dur = 0f;

        // vertex of parabolic equation
        float maxHeight = 2f * (0.5f - (1 / Constants.gravity)) + vertDistance;

        // is the player falling or jumping up to a surface? also assuming 30 degree jump angle, need to recalculate properly
        float horSpeed = navAgent.speed * Mathf.Cos(Mathf.PI / 6);
        if (vertDistance > 0) {
            dur = (horDistance / horSpeed) + (Mathf.Sqrt(2f * maxHeight / Constants.gravity));
        } else if (vertDistance == 0) {
            dur = (horDistance / horSpeed);
        } else if (vertDistance < 0) {
            dur = (horDistance / horSpeed) + (Mathf.Sqrt(2f * Mathf.Abs(vertDistance)/ Constants.gravity));
        }

        Debug.Log(string.Format("{0:0.00} H, {1:0.00} V, Duration: {2:0.00}, maxHeight: {3:0.00}", horDistance, vertDistance, dur, maxHeight));

        float n_time = 0.0f;
        while (n_time < 1.0f) {
            float yOffset = linkCurve.Evaluate(n_time) * (vertDistance != 0 ? Mathf.Clamp(Mathf.Abs(vertDistance), 0f, 10f) : 1f);

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
