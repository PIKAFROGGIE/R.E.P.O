using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class DragPlayer : MonoBehaviourPunCallbacks
{
    [Header("Struggle Settings")]
    public float struggleValue = 0f;
    public float struggleMax = 100f;
    public float struggleAddPerPress = 25f;
    public float struggleDecay = 15f;

    public bool isBeingDragged { get; private set; }

    PlayerHealthController health;
    PlayerController controller;
    Animator anim;


    void Awake()
    {
        health = GetComponent<PlayerHealthController>();
        controller = GetComponent<PlayerController>();
        anim = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine) return;
        if (!isBeingDragged) return;

        if (health.IsIncapacitated)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            struggleValue += struggleAddPerPress;
            Debug.Log("Struggle: " + struggleValue);
        }

        struggleValue = Mathf.Max(
            0f,
            struggleValue - struggleDecay * Time.deltaTime
        );

        if (struggleValue >= struggleMax)
        {
            //RPC_BreakFree();
            photonView.RPC("RPC_BreakFree",RpcTarget.All);
        }
    }

    [PunRPC]
    public void RPC_SetDragged(bool dragged)
    {
        isBeingDragged = dragged;
        struggleValue = 0f;

        if (controller != null)
            controller.enabled = !dragged;

        if (dragged)
        {
            DisablePhysics();
        }
        else
        {
            StartCoroutine(ReEnablePhysicsSafely());
        }

    }


    [PunRPC]
    void RPC_BreakFree()
    {
        isBeingDragged = false;
        struggleValue = 0f;

        if (controller != null)
            controller.enabled = true;
    }

    void DisablePhysics()
    {
        if (controller != null)
            controller.enabled = false;

        CharacterController cc = GetComponent<CharacterController>();
        if (cc != null)
            cc.enabled = false;

        Collider[] cols = GetComponentsInChildren<Collider>();
        foreach (var col in cols)
            col.enabled = false;
    }

    IEnumerator ReEnablePhysicsSafely()
    {
        yield return null;

        yield return new WaitUntil(IsGroundedSafe);

        SnapToGround();

        CharacterController cc = GetComponent<CharacterController>();
        if (cc != null)
            cc.enabled = true;

        Collider[] cols = GetComponentsInChildren<Collider>();
        foreach (var col in cols)
            col.enabled = true;

        if (controller != null)
            controller.enabled = true;
    }

    bool IsGroundedSafe()
    {
        Vector3 origin = transform.position + Vector3.up * 0.2f;
        return Physics.Raycast(origin, Vector3.down, 0.5f);
    }

    void SnapToGround()
    {
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out RaycastHit hit, 2f))
        {
            transform.position = hit.point;
        }
    }


}
