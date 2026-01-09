using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float sprintSpeed = 8f;
    public float gravityForce = 2f;
    public float jumpForce = 6f;
    public float mouseSensitivity = 2f;
    private float cameraPitch = 0f;

    public CharacterController CC;
    public Animator anim;
    public Transform cam;

    [Header("Model")]
    public Transform model;

    private Vector3 moveInput;
    public Vector3 direction;

    private bool Isstunned = false;
    public Transform groundCheckpoint;
    public LayerMask ground;


    public AudioSource walking, running, jumping;
    public bool canWalk = true, canRun = true, canJump = true, stopSound = false, isPause = false, cross = false;

    Coroutine attackCheckRoutine;

    public int layerIndex = 0;

    public float longPressDuration = 1f;    

    PhotonView PV;

    [Header("Combo Settings")]
    public float comboInputWindow = 0.5f;
    public int totalComboCount = 2;
    int upperBodyLayer;

    [Header("References")]
    public DamageObject[] punchStun;

    private int comboIndex = 0;
    private bool isAttacking = false;
    private bool inputBuffered = false;

    public float fallDelay = 0.5f;
    public bool floating = true;
    float airTimer;

    //LYY update
    [Header("Control Lock")]
    int controlLockCount = 0;
    public bool isControlLocked => controlLockCount > 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CC = GetComponent<CharacterController>();
        PV = GetComponent<PhotonView>();
        anim = GetComponentInChildren<Animator>();
        upperBodyLayer = anim.GetLayerIndex("PunchingLayer");
    }

    // Update is called once per frame
    void Update()
    {
        if (!PV.IsMine) return;

        //LYY update
        if (isControlLocked)
        {
            moveInput = Vector3.zero;

            if (walking) walking.Stop();
            if (running) running.Stop();

            anim.SetFloat("Blend", 0f);
            return;
        }

        if (CC.isGrounded)
        {
            airTimer = 0f;
            floating = false;
            anim.SetBool("IsGrounded", true);
        }
        else
        {
            airTimer += Time.deltaTime;

            anim.SetBool("IsGrounded", false);

            if(!anim.GetCurrentAnimatorStateInfo(0).IsName("Jumping"))
            {
                if (!Isstunned&&!isAttacking && airTimer >= fallDelay)
                {
                    floating = true;
                    PV.RPC("RPC_CrossFade", RpcTarget.All, "Falling", 0.1f);
                    cross = false;
                }
            }
        }

        if (Isstunned)
        {
            if (walking) walking.Stop();
            if (running) running.Stop();
            return;
        }

        float yVelocity = moveInput.y;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 camForward = cam.forward;
        Vector3 camRight = cam.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = camForward * v + camRight * h;

        direction = moveDir.normalized;

        float speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;
        moveInput = direction * speed;

        moveInput.y = yVelocity;
        moveInput.y += Physics.gravity.y * gravityForce * Time.deltaTime;

        if (CC.isGrounded)
        {
            if (!canJump)
            {
                canJump = true;
                if (!isAttacking && !cross)
                {
                    PV.RPC("RPC_CrossFade", RpcTarget.All, "MovementTree", 0.15f);
                    cross = true;
                }
            }
            moveInput.y = 0f;
            if (Input.GetKeyDown(KeyCode.Space) && canJump)
            {
                canJump = false;
                if (jumping != null)
                    jumping.Play();
                moveInput.y = jumpForce;
                PV.RPC("RPC_CrossFade", RpcTarget.All, "Jumping", 0.1f);
                cross = false;
            }
        }

        if (!Isstunned && direction.magnitude > 0.1f)
        {
            Quaternion targetRot = Quaternion.LookRotation(new Vector3(moveInput.x, 0f, moveInput.z));
            model.rotation = Quaternion.Slerp(model.rotation, targetRot,Time.deltaTime * 12f);
        }

        CC.Move(moveInput * Time.deltaTime);





        float targetBlend = 0f;

        bool isMoving = direction.magnitude > 0.1f;
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);

        if (!isMoving)
        {
            targetBlend = 0f; 
        }
        else if (isMoving && !isSprinting)
        {
            targetBlend = 0.5f;
        }
        else if (isMoving && isSprinting)
        {
            targetBlend = 1f;   
        }

        float currentBlend = anim.GetFloat("Blend");
        float smoothBlend = Mathf.Lerp(currentBlend, targetBlend, Time.deltaTime * 8f);

        if (!isAttacking)
        {
            anim.SetFloat("Blend", smoothBlend);
        }

        if (smoothBlend > 0.7f)
        {
            if (walking) walking.Stop();
            if (running && !running.isPlaying) running.Play();
        }
        else if (smoothBlend > 0.1f)
        {
            if (running) running.Stop();
            if (walking && !walking.isPlaying) walking.Play();
        }
        else
        {
            if (walking) walking.Stop();
            if (running) running.Stop();
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (attackCheckRoutine != null)
                StopCoroutine(attackCheckRoutine);

            attackCheckRoutine = StartCoroutine(CheckAttackInput());
        }
    }

    private void OnTriggerEnter(Collider other)
    {

    }

    IEnumerator DoPunch()
    {
        isAttacking = true;
        inputBuffered = false;

        comboIndex = (comboIndex % totalComboCount) + 1;
        string animName = "Box" + comboIndex;

        anim.SetInteger("comboIndex", comboIndex);
        PV.RPC("RPC_CrossFade", RpcTarget.All, animName, 0.1f);
        /*anim.CrossFade(animName, 0.1f);
        anim.CrossFade(animName, 0.1f, upperBodyLayer);*/

        if (CC != null)
            //CC.enabled = false;

        while (!anim.GetCurrentAnimatorStateInfo(0).IsName(animName) && !anim.GetCurrentAnimatorStateInfo(1).IsName(animName))
            yield return null;

        float hitStart = 0.25f;
        float hitEnd = 0.45f;

        while (anim.GetCurrentAnimatorStateInfo(0).normalizedTime < hitStart && anim.GetCurrentAnimatorStateInfo(1).normalizedTime < hitStart)
            yield return null;

        EnableDamage(comboIndex, true);

        while (anim.GetCurrentAnimatorStateInfo(0).normalizedTime < hitEnd && anim.GetCurrentAnimatorStateInfo(1).normalizedTime < hitEnd)
            yield return null;

        EnableDamage(comboIndex, false);

        float timer = comboInputWindow;
        while (timer > 0f)
        {
            if (inputBuffered)
            {
                StartCoroutine(DoPunch());
                yield break;
            }
            timer -= Time.deltaTime;
            yield return null;
        }

        comboIndex = 0;
        comboIndex = 0;

        if (CC.isGrounded)
        {
            PV.RPC("RPC_CrossFade", RpcTarget.All, "MovementTree", 0.1f);
        }
        else if(floating)
        {
            PV.RPC("RPC_CrossFade", RpcTarget.All, "Falling", 0.1f);
        }
        if (CC != null)
            CC.enabled = true;

        isAttacking = false;
    }

    public void SetStunned(bool value)
    {
        Isstunned = value;

        // CC.enabled = !value;

        if (value)
        {
            isAttacking = false;
            inputBuffered = false;
            comboIndex = 0;
        }
    }
    void EnableDamage(int index, bool enable)
    {
        for (int i = 0; i < punchStun.Length; i++)
        {
            punchStun[i].enable = true;
        }
    }

    IEnumerator CheckAttackInput()
    {
        float timer = 0f;
        while (Input.GetMouseButton(0))
        {
            timer += Time.deltaTime;

            if (timer >= longPressDuration)
            {
                yield break;
            }

            yield return null;
        }

        if (!isAttacking)
        {
            StartCoroutine(DoPunch());
        }
        else
        {
            inputBuffered = true;
        }
    }

    //LYY update
    [PunRPC]
    public void RPC_AddControlLock()
    {
        controlLockCount++;
    }

    [PunRPC]
    public void RPC_RemoveControlLock()
    {
        controlLockCount = Mathf.Max(0, controlLockCount - 1);
    }

    [PunRPC]
    public void RPC_DestroyAllBananaPeels()
    {
        BananaPeel[] peels = FindObjectsOfType<BananaPeel>();
        foreach (var peel in peels)
        {
            Destroy(peel.gameObject);
        }
    }

    [PunRPC]
    public void RPC_TempControlLock(float duration)
    {
        StartCoroutine(TempControlLockRoutine(duration));
    }

    private IEnumerator TempControlLockRoutine(float duration)
    {
        controlLockCount++;
        yield return new WaitForSeconds(duration);
        controlLockCount = Mathf.Max(0, controlLockCount - 1);
    }

    [PunRPC]
    public void RPC_CrossFade(string name, float duration)
    {
        anim.CrossFade(name, duration);
        anim.CrossFade(name, duration, upperBodyLayer);
    }
}