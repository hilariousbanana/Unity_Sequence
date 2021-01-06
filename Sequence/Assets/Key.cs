using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    private SphereCollider col;

    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<SphereCollider>();    
    }

    private void OnTriggerEnter(Collider other)
    {
        //quest.instance.gotkey = true 이런식
        Destroy(gameObject);
    }
}
