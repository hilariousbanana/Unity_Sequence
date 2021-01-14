using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertScreenController : MonoBehaviour
{
    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void DamagedAnim()
    {
        anim.SetTrigger("Damage");
    }

    public void DiedAnim()
    {
        anim.SetTrigger("Die");
    }

    public void SetTrace(bool _bTrace)
    {
        anim.SetBool("Trace", _bTrace);
    }

    public void SetRespawn(bool _bRespawn)
    {
        anim.SetBool("Respawn", _bRespawn);
    }
}
