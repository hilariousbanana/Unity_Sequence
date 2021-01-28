using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MachineRobot : Enemy
{
    private EnemyData stat;
    private Transform target;
    private Transform playerTransform;
    [SerializeField]
    private ParticleSystem Explosion;
    [SerializeField]
    private Transform HPBarPos;
    public ParticleSystem muzzle;
    private RectTransform hpBar;
    public Slider HPBar;
    private GameObject canvas;
    private Camera camera;
    private Animator anim;
    private SphereCollider col;
    public Transform HeadPos;
    private AudioSource audio;
    public STATE state = STATE.Idle;

    public bool bAction;
    public bool bWalk;
    public bool bPlayerInRange;
    private bool bPlayerCaught;
    public bool bShot;
    

    // Start is called before the first frame update
    void Start()
    {
        stat = GetComponent<EnemyData>();
        canvas = GameObject.Find("Canvas");
        hpBar = Instantiate(HPBar.gameObject, canvas.transform).GetComponent<RectTransform>();
        col = GetComponent<SphereCollider>();
        anim = GetComponent<Animator>();
        audio = GetComponent<AudioSource>();
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
                if (bPlayerInRange)
                {
                    ChangeState(STATE.InRange);
                }
                break;

            case STATE.Wander:
                break;

            case STATE.InRange:
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

            case STATE.Attack:
                if (!bPlayerInRange)
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
                if (bPlayerInRange)
                {
                    ChangeState(STATE.InRange);
                }
                break;

            case STATE.Wander:
                break;

            case STATE.InRange:
                if (!bPlayerCaught && !bShot)
                {
                    StartCoroutine(InRangeCoroutine());
                }
                break;

            case STATE.Damaged:
                StopAllCoroutines();
                StartCoroutine(HPBarCoroutine());
                break;

            case STATE.Attack:

                break;

            case STATE.Died:
                Died();
                break;
        }
    }
    IEnumerator InRangeCoroutine()
    {
        //Player방향으로 회전을 시켜주어야함
        bPlayerCaught = true;
        TracePlayer();//회전
        yield return new WaitForSeconds(3.0f);
        bPlayerCaught = false;
        ChangeState(STATE.Idle);
    }

    public override void Wander() //해당없음
    {
    }

    public override void CheckPlayerInRange()
    {
        if (bPlayerInRange)
        {
            ChangeState(STATE.InRange);
        }
    }

    void OnTriggerStay(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            playerTransform = col.gameObject.transform;
            bPlayerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(col.gameObject.tag == "Player")
        {
            bPlayerInRange = false;
        }
    }

    public override void TracePlayer()
    {
        Quaternion tempRot;
        tempRot = Quaternion.LookRotation(playerTransform.position - this.transform.position);
        this.transform.rotation = tempRot;
        Shot();
    }

    public override void Damaged(int _damage)
    {
        stat.curHp -= _damage;
        ChangeState(STATE.Damaged);
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

    IEnumerator HPBarCoroutine()
    {
        hpBar.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        hpBar.gameObject.SetActive(false);
    }

    public override void Died()
    {
        Explosion.Play();
        anim.SetBool("Die", true);

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
