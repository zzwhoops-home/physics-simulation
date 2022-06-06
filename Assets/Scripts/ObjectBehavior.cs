using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectBehavior : MonoBehaviour
{
    public GameObject deathEffect;
    public Material originalMat;
    public Material selectedMat;
    private Renderer rdr;
    private float lifeTime = 30.0f;

    // Start is called before the first frame update
    void Start()
    {
        rdr = GetComponent<Renderer>();
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
    public void SelectMat(bool disp) {
        if (disp) {
            rdr.material = selectedMat;
        } else {
            rdr.material = originalMat;
        }
    }
}