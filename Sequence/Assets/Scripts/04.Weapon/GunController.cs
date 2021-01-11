using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoSingleton<GunController>
{
    //현재 착용 중인 무기 관련
    [SerializeField]
    private Weapon curWeapon;    
    private float curFireRate;

    private bool bReload;
    private bool bfineSight;

    //사운드 이펙트
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip walkSound;

    //총기 반동 관련
    [SerializeField]
    private Vector3[] originPos;
    private Vector3 recoilBack;
    private Vector3 retroActionRecoilBack;

    //Raycast, Hit 관련
    [SerializeField]
    private Camera cam;
    [SerializeField]
    private GameObject hitEffect;
    private RaycastHit hitInfo;

    //스나이퍼 정조준 시
    [SerializeField]
    private GameObject sniperScope;
    [SerializeField]
    private GameObject weaponCamera;
    private CrosshairController crosshair;
    public float ScopeFOV;
    private float normalFOV = 60f;

    //Grenade
    [SerializeField]
    private GameObject grenadePrefab;
    [SerializeField]
    private Transform grenadeTransform;
    public float throwAngle = 45.0f;
    public float gravity = 9.8f;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        crosshair = FindObjectOfType<CrosshairController>();
        recoilBack = new Vector3(originPos[curWeapon.WeaponNum].x, originPos[curWeapon.WeaponNum].y, curWeapon.RetroActionForce);
        retroActionRecoilBack = new Vector3(curWeapon.FineSightOriginPos.x, curWeapon.FineSightOriginPos.y, curWeapon.RetroActionFineSightForce);

        WeaponManager.curWeapon = curWeapon.GetComponent<Transform>();
        WeaponManager.curWeaponAnim = curWeapon.Anim;
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

    public void CancelReload()
    {
        if(bReload)
        {
            StopAllCoroutines();
            bReload = false;
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
        crosshair.FireAnim();
        curWeapon.Anim.SetTrigger("Shot");
        curWeapon.CurBulletCount--;
        curFireRate = curWeapon.FireRate;
        PlaySFX(curWeapon.ShootSound);
        if (curWeapon.WeaponType == "Grenade")
        {
            ThrowGrenade();
        }
        else
        {
            curWeapon.Muzzle.Play();
            Hit();
            StopAllCoroutines();
            StartCoroutine(RetroActionCoroutine());
        }

    }

    private void ThrowGrenade()
    {
        StopAllCoroutines();
        StartCoroutine(InstantiateGrenadeCoroutine());
    }

    IEnumerator InstantiateGrenadeCoroutine()
    {
        yield return new WaitForSeconds(0.8f);
        //GameObject grenade = Instantiate(grenadePrefab, transform.position, transform.rotation);
        GameObject grenade = Instantiate(grenadePrefab, grenadeTransform.position, grenadeTransform.rotation);
        //Rigidbody rb = grenade.GetComponent<Rigidbody>();
        //rb.AddForce(grenadeTransform.forward * throwForce, ForceMode.Impulse);

        float target_Dist = 20f;

        //Calculate the Velocity Needed to Throw the Object to the Target at Specified Angle
        float projectile_Velo = target_Dist / (Mathf.Sin(2 * throwAngle * Mathf.Deg2Rad) / gravity);

        //Extract the X and Y Component of the Velocity
        float Velo_X = Mathf.Sqrt(projectile_Velo) *  Mathf.Cos(throwAngle * Mathf.Deg2Rad);
        float Velo_Y = Mathf.Sqrt(projectile_Velo) * Mathf.Sin(throwAngle * Mathf.Deg2Rad);

        //Flight Time
        float flightDuration = target_Dist / Velo_X;

        //Rotate Projectile to face the Target
        grenade.transform.rotation = Quaternion.LookRotation(grenade.transform.forward);

        float elapse_time = 0;

        while(elapse_time < flightDuration)
        {
            grenade.transform.Translate(0, (Velo_Y - (gravity * elapse_time)) * Time.deltaTime, Velo_X * Time.deltaTime);
            elapse_time += Time.deltaTime;
            yield return null;
        }

    }


    private void Hit()
    {
        if(Physics.Raycast(cam.transform.position, cam.transform.forward + 
            new Vector3(Random.Range(-crosshair.GetAccuracy() - curWeapon.Accuracy, crosshair.GetAccuracy() + curWeapon.Accuracy),
                               Random.Range(-crosshair.GetAccuracy() - curWeapon.Accuracy, crosshair.GetAccuracy() + curWeapon.Accuracy),
                               0),
            out hitInfo, curWeapon.Range))
        {
            Debug.DrawRay(cam.transform.position, cam.transform.forward * 10 , Color.red, 100);
            Debug.Log(hitInfo.collider.gameObject.tag);
            if(hitInfo.collider.gameObject.tag == "Enemy")
            {
                hitInfo.collider.gameObject.GetComponent<Enemy>().Damaged(curWeapon.Damage);
            }
            var clone = Instantiate(hitEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal), hitInfo.collider.gameObject.transform); //hitinfo.normal = 충돌한 표면방향
            Destroy(clone, 1.5f);
        }
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
        crosshair.FineSightAnim(bfineSight);

        if(bfineSight)
        {
            StopAllCoroutines();
            StartCoroutine(FineSightActivateCoroutine());
            if(curWeapon.WeaponType == "Sniper")
            {
                ActivateSniperScope();
            }
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(FineSightDeactivateCoroutine());
            if(curWeapon.WeaponType == "Sniper")
            {
                DeactivateSniperScope();
            }
        }
    }

    public void CancelFineSight()
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
        while (curWeapon.transform.localPosition != originPos[curWeapon.WeaponNum])
        {
            curWeapon.transform.localPosition = Vector3.Lerp(curWeapon.transform.localPosition, originPos[curWeapon.WeaponNum], 0.08f);
            yield return null;
        }
    }

    private void ActivateSniperScope()
    {
        sniperScope.SetActive(true);
        weaponCamera.SetActive(false);
        cam.fieldOfView = ScopeFOV;
    }

    private void DeactivateSniperScope()
    {
        sniperScope.SetActive(false);
        weaponCamera.SetActive(true);
        cam.fieldOfView = normalFOV;
    }

    IEnumerator RetroActionCoroutine()
    {
        if(!bfineSight)
        {
            curWeapon.transform.localPosition = originPos[curWeapon.WeaponNum];
            while(curWeapon.transform.localPosition.z <= curWeapon.RetroActionForce - 0.02f)
            {
                curWeapon.transform.localPosition = Vector3.Lerp(curWeapon.transform.localPosition, recoilBack, 0.2f);
                yield return null;
            }
            while(curWeapon.transform.localPosition != originPos[curWeapon.WeaponNum])
            {
                curWeapon.transform.localPosition = Vector3.Lerp(curWeapon.transform.localPosition, originPos[curWeapon.WeaponNum], 0.05f);
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

    public Weapon GetWeapon()
    {
        return curWeapon;
    }

    public bool GetFineSightMode()
    {
        return bfineSight;
    }

    public void GunChange(Weapon _weapon)
    {
        if(WeaponManager.curWeapon != null)
        {
            WeaponManager.curWeapon.gameObject.SetActive(false);

            curWeapon = _weapon;
            WeaponManager.curWeapon = curWeapon.GetComponent<Transform>();
            WeaponManager.curWeaponAnim = curWeapon.Anim;

            curWeapon.transform.localPosition = originPos[curWeapon.WeaponNum];
            curWeapon.gameObject.SetActive(true);
        }
    }
}
