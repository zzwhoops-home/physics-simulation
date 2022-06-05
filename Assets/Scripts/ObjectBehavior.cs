using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectBehavior : MonoBehaviour
{
    public GameObject deathEffect;

    private float lifeTime = 15.0f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Lifespan());
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("DestroyObj")) {
            KillObject();
        }
    }
    private IEnumerator Lifespan()
    {
        yield return new WaitForSeconds(lifeTime);
        KillObject();
    }
    private void KillObject() {
        GameObject go = Instantiate(deathEffect, transform.position, deathEffect.transform.rotation);
        Destroy(go, 3f);
        Destroy(gameObject);
    }
}