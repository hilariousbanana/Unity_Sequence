using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoSingleton<PlayerController>
{
    [SerializeField]
    private PlayerStat stat;
    [SerializeField]
    private Camera cam;
    private Transform respawn;

    public enum STATE
    {
        Idle,
        Damaged,
        Died, 
        Respawned
    }

    public STATE state = STATE.Idle;

    #region GetComponents(Rigidbody, Collider, Crosshair)
    private Rigidbody rigidBody;
    private CapsuleCollider collider;
    private CrosshairController crosshair;
    public AlertScreenController bloodScreen;
    #endregion

    #region Movement Variables(Walk, Run, Crouch) - Floats
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float runSpeed;
    private float applySpeed;
    [SerializeField]
    private float jumpForce;
    [SerializeField]
    private float crouchSpeed;
    [SerializeField]
    private float crouchPosY;
    private float originPosY;
    private float applyPosY;
    #endregion

    #region Movement Variables - Bools
    private bool bWalk = false;
    private bool bRun = false;
    public bool bOnGround = true;
    private bool bCrouch = false;
    private bool bDamaged = false;
    #endregion

    #region Camera Variables(Rotation, Movement) - Floats
    [SerializeField]
    private float camMoveY;

    [SerializeField]
    private float lookSensitivity;

    [SerializeField]
    private float camRotLimit;
    private float curCamRotX = 0f;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        crosshair = FindObjectOfType<CrosshairController>();
        collider = GetComponent<CapsuleCollider>();
        respawn = FindObjectOfType<StageInformation>().GetComponent<StageInformation>().RespawnPoint;

        applySpeed = walkSpeed;
        originPosY = cam.transform.localPosition.y;
        applyPosY = originPosY;
        this.transform.position = respawn.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(DialogueManager.instance.bDialEnd)
        {
            IsOnGround();
            TryJump();
            TryRun();
            TryCrouch();
            Move();
            CameraRotation();
            PlayerRotation();
            PlayerStateMachine();
        }
    }

    #region Player FSM
    void PlayerStateMachine()
    {
        switch(state)
        {
            case STATE.Idle:
                if(IsDamaged())
                {
                    ChangePlayerState(STATE.Damaged);
                }
                break;

            case STATE.Damaged:
                if(IsDied())
                {
                    ChangePlayerState(STATE.Died);
                }
                else
                {
                    ChangePlayerState(STATE.Idle);
                }
                break;

            case STATE.Died:
                if(DataController.instance.data.CurrentRespawnCount > 0)
                {
                    ChangePlayerState(STATE.Respawned);
                }
                else
                {
                    //게임오버 //스테이지 다시시작?
                }
                break;

            case STATE.Respawned:
                ChangePlayerState(STATE.Idle);
                break;

            default:
                break;
        }
    }

    void ChangePlayerState(STATE _state)
    {
        switch (_state)
        {
            case STATE.Idle:
                state = STATE.Idle;
                break;

            case STATE.Damaged:
                state = STATE.Damaged;
                Damaged();
                break;

            case STATE.Died:
                state = STATE.Died;
                Died();
                break;

            case STATE.Respawned:
                Respawn();
                DataController.instance.data.CurrentRespawnCount--;
                state = STATE.Respawned;
                break;
            default:
                break;
        }
    }

    private bool IsDamaged()
    {
        return bDamaged;
    }

    void Damaged()
    {
        Debug.Log("Damaged.");
        bloodScreen.DamagedAnim();
        bDamaged = false;
        
    }

    private bool IsDied()
    {
        if(stat.HP > 0)
        {
            return false;
        }

        return true;
    }

    void Died()
    {
        bloodScreen.DiedAnim();
    }

    private void Respawn()
    {
        StartCoroutine(RespawnCoroutine());
    }

    IEnumerator RespawnCoroutine()
    {
        bloodScreen.SetRespawn(false);
        bloodScreen.DiedAnim();
        transform.position = respawn.position;
        HUDManager.instance.RespawnBar.SetActive(true);
        yield return new WaitForSeconds(0.8f);
        HUDManager.instance.RespawnBar.SetActive(false);
        bloodScreen.SetRespawn(true);
        SetHPMax();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            bDamaged = true;
            ChangeHP(-30);
        }
    }
    #endregion

    #region (Try)Movement (Jump, Run, Crouch)
    private void IsOnGround()
    {
        bOnGround = Physics.Raycast(transform.position, Vector3.down, collider.bounds.extents.y);
        crosshair.RunAnim(!bOnGround); 
    }

    private void TryJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && bOnGround)
        {
            Jump();
        }
    }

    private void Jump()
    {
        if (bCrouch)
            Crouch();
        if (!bOnGround)
            return;
        bOnGround = false;
        rigidBody.velocity = transform.up * jumpForce;
        crosshair.RunAnim(!bOnGround);
    }

    private void TryRun()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            Run();
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            CancelRun();
        }
    }

    private void TryCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Crouch();
        }
    }

    private void Crouch()
    {
        bCrouch = !bCrouch;

        if (bCrouch)
        {
            applySpeed = crouchSpeed;
            applyPosY = crouchPosY;
            crosshair.CrouchAnim(bCrouch);
        }
        else
        {
            applySpeed = walkSpeed;
            applyPosY = originPosY;
            crosshair.CrouchAnim(bCrouch);
        }

        //cam.transform.localPosition = new Vector3(cam.transform.localPosition.x, applyPosY, cam.transform.localPosition.z);

        StartCoroutine(CrouchCoroutine());
    }

    IEnumerator CrouchCoroutine()
    {
        float posY = cam.transform.localPosition.y;
        int count = 0;

        while (posY != applyPosY)
        {
            count++;
            posY = Mathf.Lerp(posY, applyPosY, 0.01f);
            cam.transform.localPosition = new Vector3(0, posY, 0);
            if (count > 200)
                break;
            yield return null;
        }
        cam.transform.localPosition = new Vector3(0, applyPosY, 0);
    }

    private void MoveCheck(float dirX, float dirZ)
    {
        if (!bRun && !bCrouch && bOnGround)
        {
            if (dirX == 0 && dirZ == 0)
            {
                bWalk = false;
            }
            else
            {
                bWalk = true;
            }
            crosshair.WalkAnim(bWalk);
            crosshair.RunAnim(bRun);
        }

    }

    private void Move()
    {
        float moveDirX = Input.GetAxisRaw("Horizontal");
        float moveDirZ = Input.GetAxisRaw("Vertical");

        MoveCheck(moveDirX, moveDirZ);

        Vector3 moveH = transform.right * moveDirX;
        Vector3 moveV = transform.forward * moveDirZ;

        Vector3 velocity = (moveH + moveV).normalized * applySpeed;
        rigidBody.MovePosition(transform.position + velocity);
    }

    private void Run()
    {
        if (bCrouch)
            Crouch();
        bRun = true;
        applySpeed = runSpeed;
        crosshair.RunAnim(bRun);
    }

    private void CancelRun()
    {
        bRun = false;
        crosshair.RunAnim(bRun);
        applySpeed = walkSpeed;
    }

    #endregion

    #region Rotation
    private void CameraRotation()
    {
        float xRot = Input.GetAxisRaw("Mouse Y");
        float camRotX = xRot * lookSensitivity;

        curCamRotX -= camRotX;
        curCamRotX = Mathf.Clamp(curCamRotX, -camRotLimit, camRotLimit);

        cam.transform.localEulerAngles = new Vector3(curCamRotX, 0f, 0f);

    }

    private void PlayerRotation()
    {
        float yRot = Input.GetAxisRaw("Mouse X");
        Vector3 playerRotY = new Vector3(0f, yRot, 0f) * lookSensitivity;

        rigidBody.MoveRotation(rigidBody.rotation * Quaternion.Euler(playerRotY));
    }
    #endregion

    #region GetValues(bWalk, bRun, HP)
    public bool GetPlayerWalk()
    {
        return bWalk;
    }

    public bool GetPlayerRun()
    {
        return bRun;
    }

    public int GetPlayerHP()
    {
        return stat.HP;
    }
    #endregion

    #region Set/Change Values(HP, Respawn Count)
    public void SetHPMax()
    {
        stat.HP = stat.MaxHP;
    }

    public void ChangeHP(int _value)
    {
        int temp = 0;
        temp = stat.HP +_value;

        if(temp >= 100)
        {
            temp = 100;
        }
        else if(temp <= 0)
        {
            temp = 0;
        }
        stat.HP = temp;
    }

    public void SetDamaged(bool _damaged)
    {
        bDamaged = _damaged;
    }
    #endregion
}
