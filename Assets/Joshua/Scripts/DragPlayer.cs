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
            RPC_BreakFree();
            //photonView.RPC("RPC_BreakFree",RpcTarget.All);
        }
    }

    //[PunRPC]
    public void RPC_SetDragged(bool dragged)
    {
        isBeingDragged = dragged;
        struggleValue = 0f;

        if (controller != null)
            controller.enabled = !dragged;

        if (anim != null)
        {
            if (dragged)
                anim.CrossFade("Floating", 0.1f);
            else
                anim.CrossFade("MovementTree", 0.1f);
        }
    }

    //[PunRPC]
    void RPC_BreakFree()
    {
        isBeingDragged = false;
        struggleValue = 0f;

        if (controller != null)
            controller.enabled = true;
    }
}
