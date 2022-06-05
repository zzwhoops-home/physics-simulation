using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class Spawner : MonoBehaviour
{
    public GameObject[] toSpawn;
    public GameObject selected;
    public InputActionAsset actionAsset;
    public TextMeshProUGUI selectText;
    public TextMeshProUGUI infoText;
    
    private InputActionMap playerActionMap;
    private InputAction spawnAction;
    private InputAction leftClick;

    public int objectSpawnAmt;
    private Vector3 previousVel;

    void Awake(){
        playerActionMap = actionAsset.FindActionMap("PlayerActionMap");
        spawnAction = playerActionMap.FindAction("SpawnAction");
        leftClick = playerActionMap.FindAction("LeftClick");

        spawnAction.started += Spawn;
        leftClick.started += Select;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (selectText.gameObject.activeSelf && selected != null) {
            Vector3 selectedPos = selected.transform.position;
            Vector3 selectedVel = selected.GetComponent<Rigidbody>().velocity;
            Vector3 selectedAcc = selectedVel - previousVel;

            string pos = string.Format("({0.00}î, {0.00}ĵ, {0.00}k̂)", selectedPos.x, selectedPos.y, selectedPos.z);
            string vel = string.Format("({0.00}î, {0.00}ĵ, {0.00}k̂)", selectedVel.x, selectedVel.y, selectedVel.z);
            string acc = string.Format("({0.00}î, {0.00}ĵ, {0.00}k̂)", selectedAcc.x, selectedAcc.y, selectedAcc.z);
            infoText.text = pos + "\n" + vel + "\n" + acc;

            previousVel = selected.GetComponent<Rigidbody>().velocity;
        }
        else {
            selectText.gameObject.SetActive(false);
        }
    }

    private Vector3 randPos() {
        Vector3 offset = new Vector3(Random.Range(-4f, 4f), Random.Range(-2f, 2f), Random.Range(-4f, 4f));
        return offset;
    }

    private GameObject randObj() {
        return toSpawn[Random.Range(0, toSpawn.Length)];
    }

    private void Spawn(InputAction.CallbackContext ctx) {
        for (int x = 0; x < objectSpawnAmt; x++) {
            Instantiate(randObj(), transform.position + randPos(), transform.rotation);
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
