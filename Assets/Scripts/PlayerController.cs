using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    private GameObject selected;
    public InputActionAsset actionAsset;
    private InputActionMap playerActionMap;
    private InputAction leftClick;
    private InputAction movement;

    public TextMeshProUGUI selectText;
    public TextMeshProUGUI infoText;

    private Vector3 previousVel;

    void Awake() {
        playerActionMap = actionAsset.FindActionMap("PlayerActionMap");
        leftClick = playerActionMap.FindAction("LeftClick");
        movement = playerActionMap.FindAction("Movement");

        leftClick.started += Select;
    }

    void Update()
    {
        Vector2 input = movement.ReadValue<Vector2>();
        Vector3 move = new Vector3(input.x, 0, input.y);

        transform.Translate(move * Time.deltaTime * 10f);
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
    private void Select(InputAction.CallbackContext ctx) {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        
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
