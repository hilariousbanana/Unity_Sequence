using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class NormalRobot : Enemy
{
    private EnemyData stat;

    #region Nav Mesh Agent Variables
    [SerializeField]
    private Transform[] routes;
    private NavMeshAgent nav;
    private Transform target;
    private Transform playerTransform;
    private SphereCollider collider;
    #endregion

    #region Effects / Item
    [SerializeField]
    private ParticleSystem Explosion;
    [SerializeField]
    private Transform ItemDropTransform;
    [SerializeField]
    private Transform HPBarPos;
    private RectTransform hpBar;
    public Slider HPBar;
    public ParticleSystem muzzle;
    public GameObject Key;
    public GameObject HealPackA;
    public GameObject HealPackB;
    #endregion

    private AudioSource audio;
    private Animator anim;

    [SerializeField]
    private Transform HeadPos;

    private GameObject canvas;
    private Camera camera;

    STATE state = STATE.Idle;

    public bool bAction;
    public bool bWalk;
    public bool bPlayerInRange;
    public bool bShot;

    // Start is called before the first frame update
    void Start()
    {
        stat = GetComponent<EnemyData>();
        nav = GetComponent<NavMeshAgent>();
        collider = GetComponent<SphereCollider>();
        anim = GetComponent<Animator>();
        audio = GetComponent<AudioSource>();
        canvas = GameObject.Find("Canvas");
        hpBar = Instantiate(HPBar.gameObject, canvas.transform).GetComponent<RectTransform>();
        hpBar.gameObject.SetActive(false);
        camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        CheckPlayerInRange();
        EnemyState();
        Vector3 _hpPos = camera.WorldToScreenPoint(HPBarPos.position);
        hpBar.position = _hpPos;
        hpBar.gameObject.GetComponent<Slider>().value = (float)stat.curHp / (float)stat.maxHP;
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
                else
                {
                    nav.speed = 3.5f;
                    CheckRemainingDistance();
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
                if(!bShot)
                    TracePlayer();
                break;

            case STATE.Attack:

                break;

            case STATE.Damaged:
                StopAllCoroutines();
                StartCoroutine(HPBarCoroutine());
                break;

            case STATE.Died:
                Died();
                break;
        }
        Debug.Log(state);
    }

    IEnumerator IdleCoroutine()
    {
        nav.speed = 0;
        float waitTime = Random.Range(0f, 4f);
        anim.SetBool("Walk", false);
        yield return new WaitForSeconds(waitTime);
        bAction = false;
        ChangeState(STATE.Wander);
    }

    public override void Wander()
    {
        int rand = Random.Range(0, 15);
        anim.SetBool("Walk", true);
        target = routes[rand];
        nav.SetDestination(target.position);
    }

    void CheckRemainingDistance()
    {
        if (nav.remainingDistance <= 0.5f)
        { 
            ChangeState(STATE.Idle);
        }
    }

    public override void TracePlayer()
    {
        bShot = true;
        anim.SetTrigger("Shot");
        Shot();
        nav.speed = 0;
        Vector3 relativePos = playerTransform.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePos);
        transform.rotation = rotation;
        StartCoroutine(AttackPlayerCoroutine());
        //nav.SetDestination(playerTransform.position);
    }
    
    public void Shot()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(HeadPos.position, HeadPos.transform.forward * 10, out hitInfo, 10))
        {
            Debug.Log(hitInfo.collider.tag);
            Debug.DrawRay(HeadPos.transform.position, HeadPos.transform.forward * 10, Color.red, 100);
            Debug.Log(hitInfo.collider.gameObject.tag);
            if (hitInfo.collider.gameObject.tag == "Player")
            {
                hitInfo.collider.gameObject.GetComponent<PlayerController>().ChangeHP(-stat.Damage);
                hitInfo.collider.gameObject.GetComponent<PlayerController>().SetDamaged(true);
            }
        }
    }

    IEnumerator AttackPlayerCoroutine()
    {
        yield return new WaitForSeconds(1.5f);
        bShot = false;
    }

    public void PlayMuzzle()
    {
        muzzle.Play();
        audio.Play();
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
        int temp;
        temp = stat.curHp - _damage;
        if (temp <= 0)
        {
            temp = 0;
            ChangeState(STATE.Died);
        }

        stat.curHp = temp;
        ChangeState(STATE.Damaged);
    }

    IEnumerator HPBarCoroutine()
    {
        hpBar.gameObject.SetActive(true);
        nav.speed = 0;
        anim.SetTrigger("Damaged");
        yield return new WaitForSeconds(2f);
        hpBar.gameObject.SetActive(false);
    }

    public void DamagedAnimation()
    {
        nav.speed = 3.5f;
    }

    public override void Died()
    {
        //Ragdoll 효과 넣을것. +Drop Item
        nav.speed = 0;
        anim.SetTrigger("Die");
        Explosion.Play();
        HPBar.value = 0;
        int drop = Random.Range(0, 101);
        Debug.Log(drop + " " +stat.ItemDrop);
        if (drop <= stat.ItemDrop)
        {
            int item = Random.Range(0, (stat.KeyDrop + stat.HealPackADrop + stat.HealPackBDrop) + 1);
            Debug.Log(item);
            if (item <= stat.KeyDrop) // key
            {
                Debug.Log("Key");
                Instantiate(Key, ItemDropTransform.position, ItemDropTransform.rotation);
            }
            else if (item <= stat.KeyDrop + stat.HealPackADrop) //HealPackA
            {
                Debug.Log("HealPack A");
                Instantiate(HealPackA, ItemDropTransform.position, transform.rotation);
            }
            else //HealPackB
            {
                Debug.Log("HealPack B");
                Instantiate(HealPackB, ItemDropTransform.position, transform.rotation);
            }
        }
        Destroy(hpBar.gameObject, 1.5f);
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
