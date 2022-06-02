using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private Rigidbody rb;
    public float torque;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 power = transform.up * torque * NumObj();
        Debug.Log(power);

        rb.AddTorque(transform.up * torque * NumObj(), ForceMode.Force);
    }

    private float NumObj() {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Object");
        
        return Mathf.Pow(gos.Length, 0.25f);
    }
}
