using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class NormalRobot : Enemy
{
    private EnemyData stat;
    private InstantiateItem itemSpawner;
    #region Nav Mesh Agent Variables
    [SerializeField]
    private List<Transform> routes;
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

    private GameObject MinimapCanvas;
    public GameObject MinimapIcon;
    private Transform IconPos;

    [SerializeField]
    private Transform HeadPos;

    private GameObject canvas;
    private Camera camera;

    public STATE state = STATE.Idle;

    public bool bAction;
    public bool bWalk;
    public bool bPlayerInRange;
    public bool bShot;

    private DialogueManager dial;
    private EnemySpawner spawner;

    // Start is called before the first frame update
    void Start()
    {
        stat = GetComponent<EnemyData>();
        nav = GetComponent<NavMeshAgent>();
        collider = GetComponent<SphereCollider>();
        anim = GetComponent<Animator>();
        audio = GetComponent<AudioSource>();
        itemSpawner = GetComponent<InstantiateItem>();
        canvas = GameObject.Find("Canvas");
        dial = FindObjectOfType<DialogueManager>();
        hpBar = Instantiate(HPBar.gameObject, canvas.transform).GetComponent<RectTransform>();
        routes = FindObjectOfType<StageInformation>().GetComponent<StageInformation>().Routes;
        hpBar.gameObject.SetActive(false);
        camera = Camera.main;
        spawner = FindObjectOfType<EnemySpawner>();
        MinimapCanvas = GameObject.Find("MinimapCanvas");
        IconPos = Instantiate(MinimapIcon, MinimapCanvas.transform).GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        IconPos.position = this.transform.position;
        if (dial.bDialEnd)
        {
            CheckPlayerInRange();
            EnemyState();
            Vector3 _hpPos = camera.WorldToScreenPoint(HPBarPos.position);
            hpBar.position = _hpPos;
            hpBar.gameObject.GetComponent<Slider>().value = (float)stat.curHp / (float)stat.maxHP;
        }
    }

    public override void EnemyState()
    {
        switch (state)
        {
            case STATE.Idle:
                if (!bAction)
                {
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
                StopCoroutine(HPBarCoroutine());
                StartCoroutine(HPBarCoroutine());
                break;

            case STATE.Died:
                Died();
                break;
        }
    }

    IEnumerator IdleCoroutine()
    {
        bAction = true;
        anim.SetBool("Walk", false);
        nav.speed = 0;
        float waitTime = Random.Range(0f, 4f);
        yield return new WaitForSeconds(waitTime);
        ChangeState(STATE.Wander);
    }

    public override void Wander()
    {
        bAction = false;
        anim.SetBool("Walk", true);
        nav.speed = 3.5f;

        int rand = Random.Range(0, 15);
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

    public override void TracePlayer() //Player Attack
    {
        nav.speed = 0;
        Shot();
       
        Vector3 relativePos = playerTransform.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePos);
        transform.rotation = rotation;
        StartCoroutine(AttackPlayerCoroutine());
    }
    
    public void Shot()
    {
        bShot = true;
        anim.SetTrigger("Shot");

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
        yield return new WaitForSeconds(1f);
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
            stat.curHp = temp;

            ChangeState(STATE.Died);

            return;
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

    public void DamagedAnimation() //Right After Damaged Animation(Anim Event)
    {
        ChangeState(STATE.Idle);
    }

    public override void Died()
    {
        StopAllCoroutines();
        //Ragdoll 효과 넣을것.
        nav.speed = 0;
        anim.SetBool("Die", true);
        Explosion.Play();

        itemSpawner.enabled = true;

        DataController.instance.data.CurrentKillCount++;

        StartCoroutine(DieTimer());
    }

    IEnumerator DieTimer()
    {
        yield return new WaitForSeconds(2f);
        spawner.CurInstance--;
        Destroy(hpBar.gameObject);
        Destroy(this.gameObject);
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
