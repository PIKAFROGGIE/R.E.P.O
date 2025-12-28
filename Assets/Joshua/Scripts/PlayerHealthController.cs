using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerHealthController : MonoBehaviourPunCallbacks
{
    [Header("Stun Settings")]
    public float stunCount = 0f;
    public float maxStun = 100f;
    public float stunTime = 2.5f;
    public float stunDecayRate = 15f;

    [Header("HP")]
    public float health = 100f;

    [Header("References")]
    public PlayerController controller;
    public Animator anim;

    public bool isStunned { get; private set; }

    float lastHitTime;

    void Awake()
    {
        if (!controller) controller = GetComponent<PlayerController>();
        if (!anim) anim = GetComponentInChildren<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!isStunned && stunCount > 0f)
        {
            if (Time.time - lastHitTime > 0.6f)
            {
                stunCount = Mathf.Max(0f, stunCount - stunDecayRate * Time.deltaTime);
            }
        }
    }
    public void TakeHit(float damage, float stunDamage, int attackerViewId)
    {
        //if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient) return;

        lastHitTime = Time.time;

        health -= damage;
        stunCount += stunDamage;

        Debug.Log($"[TakeHit] stunCount after hit = {stunCount}");
        //photonView.RPC("RPC_SyncState", RpcTarget.All, health, stunCount);
        RPC_SyncState(health, stunCount);

        if (!isStunned && stunCount >= maxStun)
        {
            StartCoroutine(StunRoutine());
        }
    }

    //[PunRPC]
    void RPC_SyncState(float newHealth, float newStunCount)
    {
        health = newHealth;
        stunCount = newStunCount;
    }

    IEnumerator StunRoutine()
    {
        isStunned = true;

        //photonView.RPC("RPC_SetStunned", RpcTarget.All, true);
        RPC_SetStunned(true);
        yield return new WaitForSeconds(stunTime);

        stunCount = 0f;
        //photonView.RPC("RPC_SyncState", RpcTarget.All, health, stunCount);
        RPC_SyncState(health, stunCount);

        isStunned = false;
        //photonView.RPC("RPC_SetStunned", RpcTarget.All, false);
        RPC_SetStunned(false);
    }

    //[PunRPC]
    void RPC_SetStunned(bool stunned)
    {
        isStunned = stunned;

        if (controller != null)
        {
            controller.SetStunned(stunned);
        }

        if (anim != null)
        {
            anim.SetBool("Stunned", stunned);
        }
    }
}
