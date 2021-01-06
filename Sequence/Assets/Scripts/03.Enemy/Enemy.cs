using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : MonoBehaviour
{
    public enum STATE
    {
        Idle,
        Wander,
        InRange,
        Attack,
        Damaged,
        Died
    }

    public abstract void EnemyState();

    public abstract void ChangeState(STATE _state);

    public abstract void Wander();

    public abstract void TracePlayer();

    public abstract void CheckPlayerInRange();

    public abstract void Damaged(int _damage);

    public abstract void Died();
}
