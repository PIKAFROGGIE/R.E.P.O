using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class DamageObject : MonoBehaviour
{
    PlayerHealthController health;
    public PhotonView attackerPV;
    public float damage = 10f;
    public float stunDamage = 30f;

    public bool attackPlayer = true;
    public bool enable = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!enable) return;
        if (!attackPlayer) return;
        if (!other.gameObject.CompareTag("Player") )return;
        health = other.GetComponent<PlayerHealthController>();
        PhotonView targetPV = other.GetComponent<PhotonView>();

        if (health == null && targetPV == null) return;

        if (targetPV.ViewID == attackerPV.ViewID)
            return;

        if (!attackerPV.IsMine)
            return;

        Debug.Log("Hit Player at " + transform.position);

        targetPV.RPC("RPC_TakeHit",targetPV.Owner,damage,stunDamage,attackerPV.ViewID);


        enable = false;
    }
}

