using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
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

    private bool isWaiting = false;
    public bool tutorialMovement = true;
    public bool verticalmoveDetected = true;
    public bool horizontalmoveDetected = true;
    private bool Isstunned = false;
    public Transform groundCheckpoint;
    public LayerMask ground;


    public AudioSource walking, running, jumping;
    public bool canWalk = true, canRun = true, canJump = true, stopSound = false, isPause = false;



    public int layerIndex = 0;

    public float longPressDuration = 1f; // Duration to consider as a long press

    PhotonView PV;

    [Header("Combo Settings")]
    public float comboInputWindow = 0.5f;
    public int totalComboCount = 2;

    [Header("References")]
    //public DamageObject[] punchDamages;

    private int comboIndex = 0;
    private bool isAttacking = false;
    private bool inputBuffered = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CC = GetComponent<CharacterController>();
        PV = GetComponent<PhotonView>();
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!PV.IsMine) return;

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

        //moveInput = horizontalmove + verticalmove;
        //moveInput.Normalize();
        /*
        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveInput = moveInput * sprintSpeed;
        }
        else
        {
            moveInput = moveInput * moveSpeed;
        }
        */

        moveInput.y = yVelocity;
        moveInput.y += Physics.gravity.y * gravityForce * Time.deltaTime;

        if (CC.isGrounded)
        {
            canJump = true;
            moveInput.y = 0f;
            moveInput.y += Physics.gravity.y * gravityForce * Time.deltaTime;
            if (Input.GetKeyDown(KeyCode.Space) && canJump)
            {
                canJump = false;
                if (jumping != null)
                    jumping.Play();
                moveInput.y = jumpForce;
            }
        }

        if (!isAttacking && !Isstunned && direction.magnitude > 0.1f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveInput);
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
            if (!isAttacking)
            {
                StartCoroutine(DoPunch());
            }
            else
            {
                inputBuffered = true;
            }
        }

        if (Isstunned)
        {
            if (walking) walking.Stop();
            if (running) running.Stop();
            return;
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
        anim.CrossFade(animName, 0.1f);

        if (CC != null)
            //CC.enabled = false;

        while (!anim.GetCurrentAnimatorStateInfo(0).IsName(animName))
            yield return null;

        float hitStart = 0.25f;
        float hitEnd = 0.45f;

        while (anim.GetCurrentAnimatorStateInfo(0).normalizedTime < hitStart)
            yield return null;

        //EnableDamage(comboIndex, true);

        while (anim.GetCurrentAnimatorStateInfo(0).normalizedTime < hitEnd)
            yield return null;

        //EnableDamage(comboIndex, false);

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
        anim.CrossFade("MovementTree", 0.15f);

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

            anim.CrossFade("Idle", 0.1f);
        }
    }
    /*
    void EnableDamage(int index, bool enable)
    {
        for (int i = 0; i < punchDamages.Length; i++)
        {
            punchDamages[i].enable = (i == index - 1) && enable;
            punchDamages[i].attacked = false;
        }
    }

    */

}