using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    private CharacterController characterController;
    private CapsuleCollider capsuleCollider;
    private float radius;
    private Vector3 bottom, top;
    public int movementSpeed;
    public float gravity;
    public float jumpHeight;
    private Vector3 camRotation;
    public float sensitivity;
    private Vector3 move;
    private float upwardVel;
    [SerializeField] private bool grounded;
    [SerializeField] private LayerMask layerMask;

    private Camera cam;

    public InputActionAsset actionAsset;
    private InputActionMap playerActionMap;
    private InputAction leftClick;
    private InputAction movement;
    private InputAction sprint;
    private InputAction mouse;
    private InputAction jump;

    private float hitRadius = 0.33f;
    private GameObject selected;
    public TextMeshProUGUI selectText;
    public TextMeshProUGUI infoText;

    private Vector3 previousVel;
    private Vector3 previousPos;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        cam = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Awake() {
        capsuleCollider = transform.GetChild(0).GetComponent<CapsuleCollider>();
        radius = capsuleCollider.radius;

        playerActionMap = actionAsset.FindActionMap("PlayerActionMap");
        leftClick = playerActionMap.FindAction("LeftClick");
        movement = playerActionMap.FindAction("Movement");
        sprint = playerActionMap.FindAction("Sprint");
        mouse = playerActionMap.FindAction("Mouse");
        jump = playerActionMap.FindAction("Jump");

        leftClick.started += Select;
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

            if (sprint.ReadValue<float>() == 1f) { move *= 1.75f; };

        if (upwardVel < 0) {
            upwardVel = 0f;
        }
        } else {
            upwardVel -= (gravity * Time.fixedDeltaTime);
        }
        
        move.y = upwardVel;

        characterController.Move(move * Time.fixedDeltaTime);
    }

    private void OnGround() {
        top = transform.position + (transform.up * ((capsuleCollider.height / 2 - radius) + characterController.skinWidth));
        bottom = transform.position - (transform.up * ((capsuleCollider.height / 2 - radius) + characterController.skinWidth + 0.05f));

        Collider[] colliders = Physics.OverlapCapsule(bottom, top, radius, layerMask);
        if (colliders.Length != 0) {
            grounded = true;
        } else {
            grounded = false;
        }
    }

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
                selectText.gameObject.SetActive(true);
                selected = rayHit.transform.gameObject;
                DisplaySelection(true);
            }
            else {
                selectText.gameObject.SetActive(false);
            }
        }
    }
    
    private void SelectedObject() { 
        if (selectText.gameObject.activeSelf && selected != null) {
            Vector3 selectedPos = selected.transform.position;
            Vector3 selectedVel = selected.gameObject.GetComponent<Rigidbody>().velocity;
            Debug.Log(selectedVel);
            Vector3 selectedAcc = (selectedVel - previousVel) / Time.fixedDeltaTime;

            string obj = "Object: " + selected.name;
            string pos = string.Format("x: ({0:0.00}î, {1:0.00}ĵ, {2:0.00}k̂)", selectedPos.x, selectedPos.y, selectedPos.z);
            string vel = string.Format("dx/dt: ({0:0.00}î, {1:0.00}ĵ, {2:0.00}k̂) = {3:0.00}m/s", selectedVel.x, selectedVel.y, selectedVel.z, selectedVel.magnitude);
            string acc = string.Format("d<sup>2</sup>(x)/dt<sup>2</sup>: ({0:0.00}î, {1:0.00}ĵ, {2:0.00}k̂ = {3:0.00}m/s^2)", selectedAcc.x, selectedAcc.y, selectedAcc.z, selectedAcc.magnitude);
            infoText.text = obj + "\n" + pos + "\n" + vel + "\n" + acc;
            
            previousVel = selectedVel;
        } else {
            selectText.gameObject.SetActive(false);
        }
    }

    private void DisplaySelection(bool disp) {
        selected.GetComponent<ObjectBehavior>().SelectMat(disp);
    }

    private void Jump(InputAction.CallbackContext ctx) {
        // v = sqrt(2gh)
        if (grounded) {
            upwardVel += Mathf.Sqrt(2 * gravity * jumpHeight);
        }
    }

    void OnEnable() {
        playerActionMap.Enable();
    }

    void OnDisable() {
        playerActionMap.Disable();
    }
}
