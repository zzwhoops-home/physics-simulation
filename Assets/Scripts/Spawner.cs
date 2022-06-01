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

    private int cooldown = 3;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Awake(){
        spawnAction = actionsActionMap.FindAction("SpawnAction");
    }

    // Update is called once per frame
    void Update()
    {
        boolean keyPressed = spawnAction.ReadValue<boolean>();

        if (keyPressed) {
            Spawn();
        }
    }

    private void Spawn() {
        float currentTime = Time.time;

        if (currentTime >= Time.time + cooldown) {
            Instantiate(toSpawn[0], transform);
        }
    }

    void OnEnable() {
        spawnerActionMap.Enable();
    }

    void OnDisable() {
        spawnerActionMap.Disable();
    }
}
