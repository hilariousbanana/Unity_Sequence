using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField]
    private Weapon curWeapon;

    private float curFireRate;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();        
    }
    private void Update()
    {
        GunFireRateCalculate();
        TryFire();
        TryReload();
        TryPlaySound();
    }

    private void GunFireRateCalculate()
    {
        if(curFireRate > 0)
        {
            curFireRate -= Time.deltaTime;
        }
    }

    private void TryFire()
    {
        if (Input.GetMouseButtonDown(0) && curFireRate <= 0)
        {
            Fire();
        }
    }

    private void TryReload()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
    }


    private void TryPlaySound()
    {
        if (curWeapon.bPlaySFX)
            PlayShotSound();
    }
    private void Fire()
    {
        if (curWeapon.CurBulletCount > 0)
            Shoot();
        else
            Reload();
    }

    private void Shoot()
    {
        curWeapon.Anim.SetTrigger("Shot");
        curWeapon.CurBulletCount--;
        curFireRate = curWeapon.FireRate;
        //PlaySFX(curWeapon.ShootSound);
        curWeapon.Muzzle.Play();
    }

    private void Reload()
    {
        if(curWeapon.ReloadCount > 0)
        {
            curWeapon.Anim.SetTrigger("Reload");
            PlaySFX(curWeapon.ReloadSound);
            curWeapon.CurBulletCount = curWeapon.MaxBulletCount;
            curWeapon.ReloadCount--;
        }
        else
        {
            //재장전 불가 Notice Activate
        }
    }

    private void PlaySFX(AudioClip _clip)
    {
        audioSource.clip = _clip;
        audioSource.Play();
    }

    public void PlayShotSound()
    {
        PlaySFX(curWeapon.ShootSound);
        curWeapon.bPlaySFX = false;
    }
}
