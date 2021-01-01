using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public string WeaponType;
    public float Range; 
    public float Accuracy;
    public float FireRate; //연사속도
    public float ReloadTime;

    public bool bPlaySFX = false;
    public bool bWalkSound = false;

    public int Damage;

    public int MaxBulletCount;
    public int CurBulletCount;
    public int ReloadCount;

    public float RetroActionForce; //반동 세기
    public float RetroActionFineSightForce; //정조준시 반동 세기

    public Vector3 FineSightOriginPos;

    public Animator Anim;
    public AudioClip ShootSound;
    public AudioClip ReloadSound;
    public ParticleSystem Muzzle;

    void MakePlaySFX()
    {
        bPlaySFX = true;
    }

    void MakePlayWalkSound()
    {
        bWalkSound = true;
    }
}
