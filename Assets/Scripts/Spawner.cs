using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class Spawner : MonoBehaviour
{
    public GameObject[] toSpawn;
    public InputActionAsset actionAsset;
    
    private InputActionMap playerActionMap;
    private InputAction spawnAction;

    public int objectSpawnAmt;

    void Awake() {
        playerActionMap = actionAsset.FindActionMap("PlayerActionMap");
        spawnAction = playerActionMap.FindAction("SpawnAction");

        spawnAction.started += Spawn;
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

    void OnEnable() {
        playerActionMap.Enable();
    }

    void OnDisable() {
        playerActionMap.Disable();
    }
}
