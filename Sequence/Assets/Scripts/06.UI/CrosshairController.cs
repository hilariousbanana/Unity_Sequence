using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairController : MonoBehaviour
{
    [SerializeField]
    private Animator Anim;

    //크로스헤어 상태에 따른 총의 정확도
    private float gunAccuracy;

    [SerializeField]
    private GameObject crosshairHUD;

    [SerializeField]
    private GunController gunCtrl;

    public void WalkAnim(bool _walk)
    {
        Anim.SetBool("Walk", _walk);
    }
    public void RunAnim(bool _run)
    {
        Anim.SetBool("Run", _run);
    }
    public void CrouchAnim(bool _crouch)
    {
        Anim.SetBool("Crouch", _crouch);
    }

    public void FineSightAnim(bool _fineSight)
    {
        Anim.SetBool("FineSight", _fineSight);
    }

    public void FireAnim()
    {
        if(Anim.GetBool("Walk"))
        {
            Anim.SetTrigger("WalkFire");
        }
        else if(Anim.GetBool("Crouch"))
        {
            Anim.SetTrigger("CrouchFire");
        }
        else
        {
            Anim.SetTrigger("IdleFire");
        }
    }

    public float GetAccuracy()
    {
        if (Anim.GetBool("Walk"))
        {
            gunAccuracy = 0.06f;
        }
        else if (Anim.GetBool("Crouch"))
        {
            gunAccuracy = 0.015f;
        }
        else if(gunCtrl.GetFineSightMode())
        {
            gunAccuracy = 0.001f;
        }
        else
        {
            gunAccuracy = 0.035f;
        }

        return gunAccuracy;
    }
}
