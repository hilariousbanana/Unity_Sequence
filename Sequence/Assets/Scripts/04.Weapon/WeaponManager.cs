using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField]
    private Weapon curWeapon;

    private float curFireRate;

    private bool bReload;
    private bool bfineSight;

    private AudioSource audioSource;
    [SerializeField]
    private AudioClip walkSound;

    [SerializeField]
    private Vector3 originPos;
    private Vector3 recoilBack;
    private Vector3 retroActionRecoilBack;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        recoilBack = new Vector3(originPos.x, originPos.y, curWeapon.RetroActionForce);
        retroActionRecoilBack = new Vector3(curWeapon.FineSightOriginPos.x, curWeapon.FineSightOriginPos.y, curWeapon.RetroActionFineSightForce);
    }
    private void Update()
    {
        GunFireRateCalculate();
        TryWalk();
        TryRun();
        TryFire();
        TryReload();
        TryPlaySound();
        TryFineSight();
    }

    private void GunFireRateCalculate()
    {
        if(curFireRate > 0)
        {
            curFireRate -= Time.deltaTime;
        }
    }

    private void TryWalk()
    {
        curWeapon.Anim.SetBool("Walk", PlayerController.instance.GetPlayerWalk());
    }

    private void TryRun()
    {
        curWeapon.Anim.SetBool("Run", PlayerController.instance.GetPlayerRun());
        if(PlayerController.instance.GetPlayerRun())
        {
            CancelFineSight();
        }
    }

    private void TryFire()
    {
        if (Input.GetMouseButtonDown(0) && curFireRate <= 0 && !bReload)
        {
            Fire();
        }
    }

    private void TryReload()
    {
        if(Input.GetKeyDown(KeyCode.R) && !bReload)
        {
            CancelFineSight();
            StartCoroutine(ReloadCoroutine());
        }
    }


    private void TryPlaySound()
    {
        if (curWeapon.bPlaySFX)
            PlayShotSound();
        if (curWeapon.bWalkSound)
            PlayWalkSound();
    }

    private void TryFineSight()
    {
        if(Input.GetMouseButtonDown(1) && !bReload)
        {
            FineSight();
        }
    }

    private void Fire()
    {
        if(!bReload)
        {
            if (curWeapon.CurBulletCount > 0)
                Shoot();
            else
            {
                CancelFineSight();
                StartCoroutine(ReloadCoroutine());
            }
        }
    }

    private void Shoot()
    {
        curWeapon.Anim.SetTrigger("Shot");
        curWeapon.CurBulletCount--;
        curFireRate = curWeapon.FireRate;
        PlaySFX(curWeapon.ShootSound);
        curWeapon.Muzzle.Play();

        StopAllCoroutines();
        StartCoroutine(RetroActionCoroutine());
    }

    IEnumerator ReloadCoroutine()
    {
        if (curWeapon.CurBulletCount == curWeapon.MaxBulletCount)
            yield break;

        if(curWeapon.ReloadCount > 0)
        {
            bReload = true;
            curWeapon.Anim.SetTrigger("Reload");
            PlaySFX(curWeapon.ReloadSound);

            yield return new WaitForSeconds(curWeapon.ReloadTime);

            curWeapon.CurBulletCount = curWeapon.MaxBulletCount;
            curWeapon.ReloadCount--;
            bReload = false;
        }
        else
        {
            //재장전 불가 Notice Activate
            Debug.Log("Reload Count == 0");
        }
    }

    private void FineSight()
    {
        bfineSight = !bfineSight;
        curWeapon.Anim.SetBool("FineSight", bfineSight);

        if(bfineSight)
        {
            StopAllCoroutines();
            StartCoroutine(FineSightActivateCoroutine());
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(FineSightDeactivateCoroutine());
        }
    }

    private void CancelFineSight()
    {
        if (bfineSight)
            FineSight();
    }

    IEnumerator FineSightActivateCoroutine()
    {
        while(curWeapon.transform.localPosition != curWeapon.FineSightOriginPos)
        {
            curWeapon.transform.localPosition = Vector3.Lerp(curWeapon.transform.localPosition, curWeapon.FineSightOriginPos, 0.08f);
            yield return null;
        }
    }

    IEnumerator FineSightDeactivateCoroutine()
    {
        while (curWeapon.transform.localPosition != originPos)
        {
            curWeapon.transform.localPosition = Vector3.Lerp(curWeapon.transform.localPosition, originPos, 0.08f);
            yield return null;
        }
    }

    IEnumerator RetroActionCoroutine()
    {
        if(!bfineSight)
        {
            curWeapon.transform.localPosition = originPos;
            while(curWeapon.transform.localPosition.z <= curWeapon.RetroActionForce - 0.02f)
            {
                curWeapon.transform.localPosition = Vector3.Lerp(curWeapon.transform.localPosition, recoilBack, 0.2f);
                yield return null;
            }
            while(curWeapon.transform.localPosition != originPos)
            {
                curWeapon.transform.localPosition = Vector3.Lerp(curWeapon.transform.localPosition, originPos, 0.05f);
                yield return null;
            }
        }
        else
        {
            curWeapon.transform.localPosition = curWeapon.FineSightOriginPos;
            while (curWeapon.transform.localPosition.z <= curWeapon.RetroActionFineSightForce - 0.02f)
            {
                curWeapon.transform.localPosition = Vector3.Lerp(curWeapon.transform.localPosition, retroActionRecoilBack, 0.2f);
                yield return null;
            }
            while (curWeapon.transform.localPosition != curWeapon.FineSightOriginPos)
            {
                curWeapon.transform.localPosition = Vector3.Lerp(curWeapon.transform.localPosition, curWeapon.FineSightOriginPos, 0.05f);
                yield return null;
            }
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
    public void PlayWalkSound()
    {
        PlaySFX(walkSound);
        curWeapon.bWalkSound = false;
    }
}
