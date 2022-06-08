using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CannonBehavior : MonoBehaviour
{
    private GameObject player;
    public GameObject cannonball;
    public float firePower;
    public ParticleSystem charge;
    private ParticleSystem curCharge;
    public Transform offset;
    public Transform nozzleOffset;

    public InputActionAsset actionAsset;
    private InputActionMap playerActionMap;
    private InputAction rightClick;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Awake()
    {
        playerActionMap = actionAsset.FindActionMap("PlayerActionMap");
        rightClick = playerActionMap.FindAction("RightClick");

        // rightClick = new InputAction(binding: "<Mouse>/rightButton", interactions: "hold(duration=" + charge.main.startLifetime + ")");

        rightClick.started += ctx => {
            curCharge = Instantiate(charge, nozzleOffset) as ParticleSystem;
            curCharge.transform.parent = transform;
            curCharge.Play();
        };
        rightClick.performed += Fire;
        rightClick.canceled += ctx => {
            Destroy(curCharge.gameObject);
        };
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Fire(InputAction.CallbackContext ctx) {
        GameObject cb = Instantiate(cannonball, nozzleOffset.transform.position, nozzleOffset.transform.rotation) as GameObject;
        cb.GetComponent<Rigidbody>().AddForce(-transform.right * firePower, ForceMode.VelocityChange);
    }
    private void KillParticle(InputAction.CallbackContext ctx) {

    }


    void OnEnable() {
        playerActionMap.Enable();
    }

    void OnDisable() {
        playerActionMap.Disable();
    }
}
