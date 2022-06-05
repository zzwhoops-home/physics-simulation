using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public GameObject selected;
    public InputActionAsset actionAsset;
    private InputActionMap playerActionMap;
    private InputAction leftClick;

    public TextMeshProUGUI selectText;
    public TextMeshProUGUI infoText;

    private Vector3 previousVel;

    void Awake() {
        leftClick = playerActionMap.FindAction("LeftClick");
        leftClick.started += Select;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
        }
        else {
            selectText.gameObject.SetActive(false);
        }
    }
    private void Select(InputAction.CallbackContext ctx) {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        
        if (Physics.Raycast(ray, out RaycastHit rayHit)) {
            if (rayHit.transform.CompareTag("Object")) {
                selectText.gameObject.SetActive(true);
                selected = rayHit.transform.gameObject;
            }
            else {
                selectText.gameObject.SetActive(false);
            }
        }
    }    

    void OnEnable() {
        playerActionMap.Enable();
    }

    void OnDisable() {
        playerActionMap.Disable();
    }
}
