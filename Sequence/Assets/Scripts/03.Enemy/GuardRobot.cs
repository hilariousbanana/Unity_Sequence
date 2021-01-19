using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class GuardRobot : Enemy
{
    private EnemyData stat;
    [SerializeField]
    private List<Transform> routes;
    private NavMeshAgent nav;
    private Transform target;
    private Transform playerTransform;
    private SphereCollider collider;
    [SerializeField]
    private ParticleSystem Explosion;
    [SerializeField]
    private Transform HPBarPos;
    private RectTransform hpBar;
    public Slider HPBar;
    private GameObject canvas;
    private Camera camera;


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
        canvas = GameObject.Find("Canvas");
        hpBar = Instantiate(HPBar.gameObject, canvas.transform).GetComponent<RectTransform>();
        routes = FindObjectOfType<StageInformation>().GetComponent<StageInformation>().Routes;
        hpBar.gameObject.SetActive(false);
        camera = Camera.main;
        Vector3 _hpPos = camera.WorldToScreenPoint(HPBarPos.position);
        hpBar.position = _hpPos;
        hpBar.gameObject.GetComponent<Slider>().value = (float)stat.curHp / (float)stat.maxHP;
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
                if (stat.curHp <= 0)
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
                StopAllCoroutines();
                StartCoroutine(HPBarCoroutine());
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
        stat.curHp -= _damage;
        ChangeState(STATE.Damaged);
    }

    IEnumerator HPBarCoroutine()
    {
        hpBar.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        hpBar.gameObject.SetActive(false);
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
