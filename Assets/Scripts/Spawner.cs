using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Spawner : MonoBehaviour
{
    public GameObject[] toSpawn;
    public InputActionAsset actionAsset;
    
    private InputActionMap spawnerActionMap;
    private InputAction spawnAction;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Awake(){
        spawnerActionMap = actionAsset.FindActionMap("SpawnActionMap");
        spawnAction = spawnerActionMap.FindAction("SpawnAction");

        spawnAction.started += Spawn; 
    }

    // Update is called once per frame
    void Update()
    {
    }

    private Vector3 randPos() {
        Vector3 offset = new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), Random.Range(-2f, 2f));
        return offset;
    }

    private GameObject randObj () {
        return toSpawn[Random.Range(0, toSpawn.Length)];
    }

    private void Spawn(InputAction.CallbackContext ctx) {
        for (int x = 0; x < 15; x++) {
            Instantiate(randObj(), transform.position + randPos(), transform.rotation);
        }
    }

    void OnEnable() {
        spawnerActionMap.Enable();
        spawnAction.Enable();

    }

    void OnDisable() {
        spawnerActionMap.Disable();
        spawnAction.Disable();
    }
}
