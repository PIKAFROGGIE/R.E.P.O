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
    public Transform camTrans;

    private Vector3 moveInput;
    public Vector3 direction;

    private bool isWaiting = false;
    public bool tutorialMovement = true;
    public bool verticalmoveDetected = true;
    public bool horizontalmoveDetected = true;
    public Transform groundCheckpoint;
    public LayerMask ground;


    public AudioSource walking, running, jumping;
    public bool canWalk = true, canRun = true, canJump = true, stopSound = false, isPause = false;


    //public GameObject bullet;

    public int layerIndex = 0;

    public float longPressDuration = 1f; // Duration to consider as a long press

    public float fireRate = 0.1f;        // Time between shots
    public float bulletSpeed = 20f;
    public bool isShooting = false;
    PhotonView PV;
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

        Vector3 verticalmove = transform.forward * Input.GetAxis("Vertical");
        Vector3 horizontalmove = transform.right * Input.GetAxis("Horizontal");

        moveInput = horizontalmove + verticalmove;
        moveInput.Normalize();
        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveInput = moveInput * sprintSpeed;
        }
        else
        {
            moveInput = moveInput * moveSpeed;
        }


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

        CC.Move(moveInput * Time.deltaTime);
        Vector2 mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSensitivity;
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y + mouseInput.x, 0f);
        cameraPitch -= mouseInput.y;
        cameraPitch = Mathf.Clamp(cameraPitch, -75f, 75f);
        camTrans.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);

        direction = moveInput;
        direction.y = 0;

        bool isMoving = direction.magnitude > 0.1f;
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);

        if (isMoving && !isSprinting)
        {
            canWalk = false;
            canRun = true;
            if (running != null)
                running.Stop();
            if (walking != null)
                walking.Play();
            if (anim != null)
            {
                anim.SetBool("walk", true);
                anim.SetBool("run", false);
            }
        }

        else if (isMoving && isSprinting)
        {
            canWalk = true;
            canRun = false;
            if (walking != null)
                walking.Stop();
            if (running != null)
                running.Play();
            if (anim != null)
            {
                anim.SetBool("walk", false);
                anim.SetBool("run", true);
            }
        }
        else
        {
            canWalk = true;
            canRun = true;
            if (running != null)
                running.Stop();
            if (walking != null)
                walking.Stop();
            if (anim != null)
            {
                anim.SetBool("walk", false);
                anim.SetBool("run", false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {

    }






}