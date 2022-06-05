using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    private CharacterController characterController;
    public int movementSpeed;
    private float velocity;
    public float gravity = -9.81f;
    private Vector3 camRotation;
    public float sensitivity;

    private Camera cam;

    public InputActionAsset actionAsset;
    private InputActionMap playerActionMap;
    private InputAction leftClick;
    private InputAction movement;
    private InputAction sprint;
    private InputAction mouse;

    private GameObject selected;
    public TextMeshProUGUI selectText;
    public TextMeshProUGUI infoText;

    private Vector3 previousVel;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        cam = Camera.main;
    }

    void Awake() {
        playerActionMap = actionAsset.FindActionMap("PlayerActionMap");
        leftClick = playerActionMap.FindAction("LeftClick");
        movement = playerActionMap.FindAction("Movement");
        sprint = playerActionMap.FindAction("Sprint");
        mouse = playerActionMap.FindAction("Mouse");

        leftClick.started += Select;
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
        Vector2 input = movement.ReadValue<Vector2>();
        Vector3 horizontal = input.x * cam.transform.right;
        Vector3 vertical = input.y * cam.transform.forward;
        Vector3 move = (horizontal + vertical) * movementSpeed * Time.deltaTime; 

        if (sprint.ReadValue<float>() == 1f) { move *= 1.75f; };

        characterController.Move(move);

        if (characterController.isGrounded) {
            velocity = 0;
        } else {
            velocity += gravity * Time.deltaTime;
            Vector3 upwardMove = new Vector3(0, velocity, 0);
            characterController.Move(upwardMove);
        }
    }

    private void MoveCamera() {
        Vector2 mouseInput = mouse.ReadValue<Vector2>() * sensitivity;
        camRotation.x = Mathf.Clamp(camRotation.x - mouseInput.y, -90f, 90f);
        camRotation.y += mouseInput.x;

        cam.transform.eulerAngles = camRotation;
    }

    private void Select(InputAction.CallbackContext ctx) {
        Vector3 pos = new Vector3(Screen.width / 2, 0, Screen.height / 2);
        Ray ray = Camera.main.ScreenPointToRay(pos);
        Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);
        
        if (Physics.Raycast(ray, out RaycastHit rayHit)) {
            if (rayHit.transform.CompareTag("Object")) {
                selectText.gameObject.SetActive(true);
                selected = rayHit.transform.gameObject;
                DisplaySelection(true);
            }
            else {
                selectText.gameObject.SetActive(false);
                if (selected != null) {
                    DisplaySelection(false);
                }
            }
        }
    }
    
    private void DisplaySelection(bool disp) {
        selected.GetComponent<ObjectBehavior>().SelectMat(disp);
    }

    void OnEnable() {
        playerActionMap.Enable();
    }

    void OnDisable() {
        playerActionMap.Disable();
    }
}
