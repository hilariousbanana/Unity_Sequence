using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoSingleton<WeaponManager>
{
    public static bool bChangeWeapon;

    [SerializeField]
    private float changeWeaponDelay;
    [SerializeField]
    private float changeWeaponEndDelay;

    [SerializeField]
    private Weapon[] weapons;
    [SerializeField]
    //private Hand[] hands;
    private Dictionary<string, Weapon> weaponDict = new Dictionary<string, Weapon>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
