using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
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


    [SerializeField]
    private float camMoveY;

    private bool bRun = false;
    private bool bOnGround = true;
    private bool bCrouch = false;

    [SerializeField]
    private float lookSensitivity;

    [SerializeField]
    private float camRotLimit;
    private float curCamRotX = 0f;

    [SerializeField]
    private Camera cam;

    private Rigidbody rigidBody;
    private CapsuleCollider collider;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        applySpeed = walkSpeed;
        collider = GetComponent<CapsuleCollider>();
        originPosY = cam.transform.localPosition.y;
        applyPosY = originPosY;
    }

    // Update is called once per frame
    void Update()
    {
        IsOnGround();
        TryJump();
        TryRun();
        TryCrouch();
        Move();
        CameraRotation();
        PlayerRotation();
    }

    private void IsOnGround()
    {
        bOnGround = Physics.Raycast(transform.position, Vector3.down, collider.bounds.extents.y + 0.1f);
    }

    private void TryJump()
    {
        if(Input.GetKeyDown(KeyCode.Space) && bOnGround)
        {
            Jump();
        }
    }

    private void Jump()
    {
        if (bCrouch)
            Crouch();
        rigidBody.velocity = transform.up * jumpForce;
    }

    private void TryRun()
    {
        if(Input.GetKey(KeyCode.LeftShift))
        {
            Run();
        }
        if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            CancelRun();
        }
    }

    private void TryCrouch()
    {
        if(Input.GetKeyDown(KeyCode.LeftControl))
        {
            Crouch();
        }
    }

    private void Crouch()
    {
        bCrouch = !bCrouch;

        if(bCrouch)
        {
            applySpeed = crouchSpeed;
            applyPosY = crouchPosY;
        }
        else
        {
            applySpeed = walkSpeed;
            applyPosY = originPosY;
        }

        //cam.transform.localPosition = new Vector3(cam.transform.localPosition.x, applyPosY, cam.transform.localPosition.z);

        StartCoroutine(CrouchCoroutine());
    }

    IEnumerator CrouchCoroutine()
    {
        float posY = cam.transform.localPosition.y;
        int count = 0;

        while(posY != applyPosY)
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

    private void Move()
    {
        float moveDirX = Input.GetAxisRaw("Horizontal");
        float moveDirZ = Input.GetAxisRaw("Vertical");

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
    }

    private void CancelRun()
    {
        bRun = false;
        applySpeed = walkSpeed;
    }

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
}
