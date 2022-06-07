using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Obstacle : MonoBehaviour
{
    private Rigidbody[] rbs;
    public TextMeshProUGUI counter;
    public float torque;

    // Start is called before the first frame update
    void Start()
    {
        rbs = Array.FindAll(GetComponentsInChildren<Rigidbody>(), child => child != GetComponent<Rigidbody>());
        foreach (Rigidbody rb in rbs) {
            rb.maxAngularVelocity = 15f;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float objMult = NumObj();
        foreach (Rigidbody rb in rbs) {
            Vector3 power = transform.up * torque * objMult;
            rb.AddTorque(transform.up * torque * objMult, ForceMode.VelocityChange);
        }
    }

    private float NumObj() {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Object");

        counter.text = "Objects on Screen: " + gos.Length;
        
        return Mathf.Pow(gos.Length, 0.25f);
    }
}
