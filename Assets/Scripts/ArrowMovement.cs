using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowMovement : MonoBehaviour
{
    private float yPos;
    [SerializeField] private float amplitude = 0.3f;
    [SerializeField] private Transform original;
    [SerializeField] private float yOffset = 0.5f;

    // Update is called once per frame
    void Update()
    {
        yPos = (Mathf.Sin(Time.time) * amplitude) + yOffset;
        transform.position = new Vector3(original.position.x, yPos, original.position.z);
    }
}
