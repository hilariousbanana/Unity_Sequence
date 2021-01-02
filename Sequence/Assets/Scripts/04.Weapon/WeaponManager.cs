using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoSingleton<WeaponManager>
{
    public static bool bChangeWeapon;

    public static Transform curWeapon;
    public static Animator curWeaponAnim;

    [SerializeField]
    private float changeWeaponDelay;
    [SerializeField]
    private float changeWeaponEndDelay;

    [SerializeField]
    private Weapon[] weapons;
    private Dictionary<int, Weapon> weaponDict = new Dictionary<int, Weapon>();

    [SerializeField]
    private int curWeaponType;
    [SerializeField]
    private GunController gunCtrl;

    [SerializeField]
    private AudioClip changeSound;
    private AudioSource audio;

    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
        audio.clip = changeSound;
        for (int i = 0; i < weapons.Length; i++)
        {
            weaponDict.Add(weapons[i].WeaponNum, weapons[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!bChangeWeapon)
        {
            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                StartCoroutine(ChangeWeaponCoroutine(0));
             }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                StartCoroutine(ChangeWeaponCoroutine(1));
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                StartCoroutine(ChangeWeaponCoroutine(2));
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                StartCoroutine(ChangeWeaponCoroutine(3));
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                StartCoroutine(ChangeWeaponCoroutine(4));
            }

        }
    }

    IEnumerator ChangeWeaponCoroutine(int _weaponNum)
    {
        bChangeWeapon = true;
        curWeaponAnim.SetTrigger("Hide");
        audio.clip = changeSound;
        audio.Play();

        yield return new WaitForSeconds(changeWeaponDelay);

        //CancelPreWeaponAction();
        WeaponChange(_weaponNum);

        yield return new WaitForSeconds(changeWeaponEndDelay);

        curWeaponType = _weaponNum;

        bChangeWeapon = false;
    }

    private void CancelPreWeaponAction()
    {
        gunCtrl.CancelFineSight();
        gunCtrl.CancelReload();
    }

    private void WeaponChange(int _weaponNum)
    {
        gunCtrl.GunChange(weaponDict[_weaponNum]);
    }
}
