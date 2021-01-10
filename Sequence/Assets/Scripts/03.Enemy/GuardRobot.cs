using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuardRobot : Enemy
{
    private EnemyData stat;
    [SerializeField]
    private Transform[] routes;
    private NavMeshAgent nav;
    private Transform target;
    private Transform playerTransform;
    private SphereCollider collider;
    [SerializeField]
    private ParticleSystem Explosion;

    STATE state = STATE.Idle;

    public bool bAction;
    public bool bWalk;
    public bool bPlayerInRange;
    private bool bPlayerCaught;

    // Start is called before the first frame update
    void Start()
    {
        stat = GetComponent<EnemyData>();
        nav = GetComponent<NavMeshAgent>();
        collider = GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckPlayerInRange();
        EnemyState();
    }

    public override void EnemyState()
    {
        switch (state)
        {
            case STATE.Idle:
                if (!bAction)
                {
                    bAction = true;
                    StartCoroutine(IdleCoroutine());
                }
                if (bPlayerInRange)
                {
                    ChangeState(STATE.InRange);
                }
                break;

            case STATE.Wander:
                if (bPlayerInRange && !bPlayerCaught)
                {
                    ChangeState(STATE.InRange);
                }
                break;

            case STATE.InRange:
                if (!bPlayerInRange)
                {
                    ChangeState(STATE.Idle);
                }
                break;

            case STATE.Damaged:
                if (stat.Hp <= 0)
                {
                    ChangeState(STATE.Died);
                }
                else
                {
                    ChangeState(STATE.Idle);
                }
                break;

            case STATE.Died:
                break;
        }
    }

    public override void ChangeState(STATE _state)
    {
        state = _state;

        switch (_state)
        {
            case STATE.Idle:
                break;

            case STATE.Wander:
                Wander();
                break;

            case STATE.InRange:
                if(!bPlayerCaught)
                {
                    StartCoroutine(InRangeCoroutine());
                    Debug.Log("Caught.");
                    //Quest에 잡힌 횟수 ++
                }
                break;

            case STATE.Damaged:
                break;

            case STATE.Died:
                Died();
                break;
        }
    }

    IEnumerator IdleCoroutine()
    {
        float waitTime = Random.Range(0f, 4f);
        yield return new WaitForSeconds(waitTime);
        bAction = false;
        ChangeState(STATE.Wander);
    }

    IEnumerator InRangeCoroutine()
    {
        bPlayerCaught = true;
        yield return new WaitForSeconds(4.0f);
        bPlayerCaught = false;
        ChangeState(STATE.Idle);
    }

    public override void Wander()
    {
        int rand = Random.Range(0, 15);

        target = routes[rand];
        nav.SetDestination(target.position);

        ChangeState(STATE.Idle);
    }

    public override void CheckPlayerInRange()
    {
        if (bPlayerInRange)
        {
            ChangeState(STATE.InRange);
        }
    }

    public override void TracePlayer()
    {
        
    }

    public override void Damaged(int _damage)
    {
        stat.Hp -= _damage;
        ChangeState(STATE.Damaged);
    }

    public override void Died()
    {
        //Ragdoll 효과 넣을것. +Drop Item
        //ChangeState(STATE.Idle);
        nav.speed = 0;
        Explosion.Play();

        Destroy(this.gameObject, 1.5f);
    }

    #region SetValues(PlayerInRange, PlayerTransform)
    public override void SetPlayerInRange(bool _IsInRange)
    {
        bPlayerInRange = _IsInRange;
    }

    public override void SetPlayerTransform(Transform _player)
    {
        playerTransform = _player;
    }
    #endregion
}
