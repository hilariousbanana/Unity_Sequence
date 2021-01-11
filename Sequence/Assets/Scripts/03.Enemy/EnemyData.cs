using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyData : MonoBehaviour
{
    public string RobotType;
    public int curHp;
    public int maxHP;
    public int Damage;
    public int ItemDrop;
    public int KeyDrop;
    public int HealPackADrop;
    public int HealPackBDrop;
}
