using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.AI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    private CharacterController characterController;
    private CapsuleCollider capsuleCollider;
    private float radius;
    private Vector3 bottom, top;
    [SerializeField] private int movementSpeed;
    [SerializeField] private float jumpHeight;
    private Vector3 camRotation;
    [SerializeField] private float sensitivity;
    private Vector3 move;
    private float upwardVel;
    [SerializeField] private bool grounded;
    [SerializeField] private LayerMask isGroundedLayerMask;
    [SerializeField] private LayerMask navPointLayerMask;

    private Camera cam;

    [SerializeField] private Transform nozzleOffset;
    [SerializeField] private GameObject navPoint;
    private GameObject curNavPoint;
    private NavMeshDemo navMeshDemo;

    [SerializeField] private InputActionAsset actionAsset;
    private InputActionMap playerActionMap;
    private InputAction leftClick;
    private InputAction middleClick;
    private InputAction movement;
    private InputAction sprint;
    private InputAction mouse;
    private InputAction jump;

    private float hitRadius = 0.33f;
    private GameObject selected;
    [SerializeField] private GameObject selectTextObject;
    [SerializeField] private TextMeshProUGUI selectText;
    [SerializeField] private TextMeshProUGUI infoText;

    private Vector3 previousVel;
    private Vector3 previousPos;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        cam = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;

        navMeshDemo = GameObject.FindGameObjectWithTag("NavMeshAgent").GetComponent<NavMeshDemo>();
    }

    void Awake() {
        capsuleCollider = transform.GetChild(0).GetComponent<CapsuleCollider>();
        radius = capsuleCollider.radius;

        playerActionMap = actionAsset.FindActionMap("PlayerActionMap");
        leftClick = playerActionMap.FindAction("LeftClick");
        middleClick = playerActionMap.FindAction("MiddleClick");
        movement = playerActionMap.FindAction("Movement");
        sprint = playerActionMap.FindAction("Sprint");
        mouse = playerActionMap.FindAction("Mouse");
        jump = playerActionMap.FindAction("Jump");

        leftClick.started += Select;
        middleClick.started += NavPoint;
        jump.started += Jump;
    }

    void Update()
    {
        MoveCamera();
    }

    void FixedUpdate()
    {
        OnGround();
        MovePlayer();

        SelectedObject();
    }

    private void MovePlayer() {
        if (grounded) {
            Vector2 input = movement.ReadValue<Vector2>();
            if (input.magnitude == 0) {
                move = Vector3.zero;
            } else {
                Vector3 horizontal = input.x * cam.transform.right;
                Vector3 forward = input.y * cam.transform.forward;
                move = (horizontal + forward) * movementSpeed;
            }

            if (sprint.ReadValue<float>() == 1f) {
                move *= 1.75f;
                Camera.main.fieldOfView = 60f;
            } else { 
                Camera.main.fieldOfView = 60f;
            }

            if (upwardVel < 0) {
                upwardVel = 0f;
            }
        } else {
            move = characterController.velocity;
            upwardVel -= (Constants.gravity * Time.fixedDeltaTime);
        }
        move.y = upwardVel;

        characterController.Move(move * Time.fixedDeltaTime);
    }

    private void Jump(InputAction.CallbackContext ctx) {
        // v = sqrt(2gh)
        if (grounded) {
            upwardVel += Mathf.Sqrt(2 * Constants.gravity * jumpHeight);
        }
    }

    // capsulecast to check if the player's collider is touching the ground
    private void OnGround() {
        top = transform.position + (transform.up * ((capsuleCollider.height / 2 - radius) + characterController.skinWidth));
        bottom = transform.position - (transform.up * ((capsuleCollider.height / 2 - radius) + characterController.skinWidth + 0.05f));

        Collider[] colliders = Physics.OverlapCapsule(bottom, top, radius, isGroundedLayerMask);
        if (colliders.Length != 0) {
            grounded = true;
        } else {
            grounded = false;
        }
    }

    // f, clamp to vertical.
    private void MoveCamera() {
        Vector2 mouseInput = mouse.ReadValue<Vector2>() * sensitivity;
        camRotation.x = Mathf.Clamp(camRotation.x - mouseInput.y, -90f, 90f);
        camRotation.y += mouseInput.x;

        cam.transform.eulerAngles = camRotation;
    }

    private void Select(InputAction.CallbackContext ctx) {
        Vector3 pos = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        Ray ray = Camera.main.ScreenPointToRay(pos);
        
        if (Physics.SphereCast(ray, hitRadius, out RaycastHit rayHit)) {
            if (selected != null) {
                DisplaySelection(false);
            }
            if (rayHit.transform.CompareTag("Object")) {
                selectTextObject.SetActive(true);
                selected = rayHit.transform.gameObject;
                DisplaySelection(true);
            }
            else {
                selectTextObject.SetActive(false);
            }
        }
    }
    
    // get information about the selected object and update information box
    private void SelectedObject() { 
        if (selectTextObject.activeSelf && selected != null) {
            Rigidbody rb = selected.GetComponent<Rigidbody>();

            Vector3 selectedPos = selected.transform.position;
            Vector3 selectedVel = rb.velocity;
            Debug.Log(selectedVel);
            Vector3 selectedAcc = (selectedVel - previousVel) / Time.fixedDeltaTime;
            // F = ma
            Vector3 selectedForce = rb.mass * selectedAcc;
            // P = mv
            Vector3 selectedMNTM = rb.mass * selectedVel;
            // T = 1/2mv^2
            float selectedKE = 0.5f * rb.mass * Mathf.Pow(selectedVel.magnitude, 2);
            // U = mgh - maybe replace 7.5 with an actual ground level
            float selectedPE = rb.mass * Constants.gravity * (selectedPos.y + 7.5f);

            string obj = "Object: " + selected.name;
            string pos = string.Format("x: ({0:0.00}î, {1:0.00}ĵ, {2:0.00}k̂)", selectedPos.x, selectedPos.y, selectedPos.z);
            string vel = string.Format("dx/dt: ({0:0.00}î, {1:0.00}ĵ, {2:0.00}k̂) = {3:0.00}m/s", selectedVel.x, selectedVel.y, selectedVel.z, selectedVel.magnitude);
            string acc = string.Format("d<sup>2</sup>(x)/dt<sup>2</sup>: ({0:0.00}î, {1:0.00}ĵ, {2:0.00}k̂ = {3:0.00}m/s^2)", selectedAcc.x, selectedAcc.y, selectedAcc.z, selectedAcc.magnitude);
            string force = string.Format("F: ({0:0.00}î, {1:0.00}ĵ, {2:0.00}k̂ = {3:0.00}N)", selectedForce.x, selectedForce.y, selectedForce.z, selectedForce.magnitude);
            string mntm = string.Format("P: ({0:0.00}î, {1:0.00}ĵ, {2:0.00}k̂ = {3:0.00}N*s)", selectedMNTM.x, selectedMNTM.y, selectedMNTM.z, selectedMNTM.magnitude);
            string ke = string.Format("KE = {0:0.00}J", selectedKE);
            string pe = string.Format("PE = {0:0.00}J", selectedPE);
            infoText.text = obj + "\n" + pos + "\n" + vel + "\n" + acc + "\n" + force + "\n" + mntm + "\n" + ke + "\n" + pe;
            
            previousVel = selectedVel;
        } else {
            selectTextObject.SetActive(false);
        }
    }

    private void DisplaySelection(bool disp) {
        selected.GetComponent<ObjectBehavior>().SelectMat(disp);
    }

    private void NavPoint(InputAction.CallbackContext ctx) {
        Vector3 pos = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        Ray ray = Camera.main.ScreenPointToRay(pos);

        if (Physics.Raycast(ray, out RaycastHit rayHit, 69f, navPointLayerMask)) {
            if (curNavPoint != null) {
                Destroy(curNavPoint);
            }
            curNavPoint = Instantiate(navPoint, nozzleOffset.transform) as GameObject;
            LineRenderer curLineRenderer = curNavPoint.GetComponent<LineRenderer>();

            curLineRenderer.SetPosition(0, nozzleOffset.position);
            curLineRenderer.SetPosition(1, rayHit.point);
            Destroy(curNavPoint, 3f);

            navMeshDemo.MoveAgent(rayHit.point);
        }
    }

    void OnEnable() {
        playerActionMap.Enable();
    }

    void OnDisable() {
        playerActionMap.Disable();
    }
}
