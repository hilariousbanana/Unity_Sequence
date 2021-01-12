using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodScreenController : MonoBehaviour
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
}
