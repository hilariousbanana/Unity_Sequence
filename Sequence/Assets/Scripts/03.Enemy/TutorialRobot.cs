using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TutorialRobot : Enemy
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
    [SerializeField]
    private Transform ItemDropTransform;
    public GameObject Key;
    public GameObject HealPackA;
    public GameObject HealPackB;
    private Animation animation;

    STATE state = STATE.Idle;

    public bool bAction;
    public bool bWalk;
    public bool bPlayerInRange;

    // Start is called before the first frame update
    void Start()
    {
        stat = GetComponent<EnemyData>();
        nav = GetComponent<NavMeshAgent>();
        collider = GetComponent<SphereCollider>();
        animation = GetComponent<Animation>();
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
                if (bPlayerInRange)
                {
                    ChangeState(STATE.InRange);
                }
                break;

            case STATE.InRange:
                if (!bPlayerInRange)
                {
                    ChangeState(STATE.Idle);
                }
                else
                {
                    //플레이어 발견 시
                    ChangeState(STATE.Attack);
                }
                break;

            case STATE.Attack:
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
                TracePlayer();

                break;

            case STATE.Attack:
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

    public override void Wander()
    {
        int rand = Random.Range(0, 15);

        target = routes[rand];
        nav.SetDestination(target.position);

        ChangeState(STATE.Idle);
    }

    public override void TracePlayer()
    {
        nav.SetDestination(playerTransform.position);
    }

    public override void CheckPlayerInRange()
    {
        if (bPlayerInRange)
        {
            ChangeState(STATE.InRange);
        }
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
        animation.Stop();
        Explosion.Play();
        int drop = Random.Range(0, 11);
        if (drop * 10 <= stat.ItemDrop)
        {
            int item = Random.Range(0, (stat.KeyDrop + stat.HealPackADrop + stat.HealPackBDrop) + 1);
            Debug.Log(item);
            if (item <= stat.KeyDrop) // key
            {
                Instantiate(Key, ItemDropTransform.position, ItemDropTransform.rotation);
            }
            else if (item <= stat.KeyDrop + stat.HealPackADrop) //HealPackA
            {
                Instantiate(HealPackA, ItemDropTransform.position, transform.rotation);
            }
            else //HealPackB
            {
                Instantiate(HealPackB, ItemDropTransform.position, transform.rotation);

            }
        }

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
