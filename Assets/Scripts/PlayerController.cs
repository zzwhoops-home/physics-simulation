using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    private CharacterController characterController;
    public int movementSpeed;
    public float gravity;
    public float jumpHeight;
    private Vector3 camRotation;
    public float sensitivity;
    private Vector3 move;
    private float upwardVel;
    [SerializeField] private bool grounded;

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

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        cam = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Awake() {
        playerActionMap = actionAsset.FindActionMap("PlayerActionMap");
        leftClick = playerActionMap.FindAction("LeftClick");
        movement = playerActionMap.FindAction("Movement");
        sprint = playerActionMap.FindAction("Sprint");
        mouse = playerActionMap.FindAction("Mouse");
        jump = playerActionMap.FindAction("Jump");

        leftClick.started += Select;
        jump.performed += Jump;
    }

    void Update()
    {
        MovePlayer();
        MoveCamera();
    }

    void FixedUpdate()
    {
        if (selectText.gameObject.activeSelf && selected != null) {
            Vector3 selectedPos = selected.transform.position;
            Vector3 selectedVel = selected.GetComponent<Rigidbody>().velocity;
            Vector3 selectedAcc = selectedVel - previousVel;

            string pos = string.Format("x: ({0:0.00}î, {0:0.00}ĵ, {0:0.00}k̂)", selectedPos.x, selectedPos.y, selectedPos.z);
            string vel = string.Format("dx/dt: ({0:0.00}î, {0:0.00}ĵ, {0:0.00}k̂)", selectedVel.x, selectedVel.y, selectedVel.z);
            string acc = string.Format("d^2(x)/dt^2: ({0:0.00}î, {0:0.00}ĵ, {0:0.00}k̂)", selectedAcc.x, selectedAcc.y, selectedAcc.z);
            infoText.text = pos + "\n" + vel + "\n" + acc;
            
            previousVel = selected.GetComponent<Rigidbody>().velocity;
        } else {
            selectText.gameObject.SetActive(false);
        }
    }

    private void MovePlayer() {
        grounded = characterController.isGrounded;

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
                upwardVel = 0;
            }
        } else {
            upwardVel -= (gravity * Time.deltaTime);
        }
        
        move.y = upwardVel;

        characterController.Move(move * Time.deltaTime);
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
