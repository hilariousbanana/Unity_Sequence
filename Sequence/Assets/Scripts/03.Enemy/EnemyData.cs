using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Robot", menuName = "Scriptable Object/Robot", order = 0)]
public class EnemyData : ScriptableObject
{
    public string RobotType;
    public int Hp;
    public int Damage;
    public float SightRange;
    public float MoveSpeed;
    public float ItemDrop;
    public float CoolDown;
}
