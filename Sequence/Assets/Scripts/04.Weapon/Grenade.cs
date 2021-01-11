using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public float delay = 3f;
    public GameObject ExplosionEffect;
    public float radius;

    float countdown;
    bool bExploded = false;
    // Start is called before the first frame update
    void Start()
    {
        countdown = delay;
    }

    // Update is called once per frame
    void Update()
    {
        countdown -= Time.deltaTime;

        if(countdown <= 0f && !bExploded)
        {
            Explode();
            bExploded = true;
        }
    }

    void Explode()
    {
        //Show Effect
        Instantiate(ExplosionEffect, transform.position, transform.rotation);

        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider nearbyObject in colliders)
        {
            if(nearbyObject.gameObject.tag == "Enemy")
            {
                nearbyObject.gameObject.GetComponent<Enemy>().Damaged(-50);
            }
        }

        Destroy(gameObject, 2f);
    }
}
