﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoSingleton<PlayerController>
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

    private bool bWalk = false;
    private bool bRun = false;
    public bool bOnGround = true;
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
    private CrosshairController crosshair;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        crosshair = FindObjectOfType<CrosshairController>();
        collider = GetComponent<CapsuleCollider>();

        applySpeed = walkSpeed;
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

    public bool GetPlayerWalk()
    {
        return bWalk;
    }

    public bool GetPlayerRun()
    {
        return bRun;
    }

    public void EndJump()
    {
        bOnGround = true;
        crosshair.RunAnim(bOnGround);
    }
}
