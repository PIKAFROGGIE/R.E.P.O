using UnityEngine;
using Photon.Pun;

public class PlungerSkill : MonoBehaviourPun
{
    public GameObject plungerAttackPrefab;
    public Transform handPoint;

    [Header("Plunger Settings")]
    public float pullForce = 10f;

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip shootSFX;
    public AudioClip hitSFX;

    public void Activate()
    {
        if (!photonView.IsMine) return;

        photonView.RPC(
        nameof(RPC_PlayShootSFX),
        RpcTarget.All
        );

        photonView.RPC(
            nameof(RPC_UsePlunger),
            RpcTarget.All,
            photonView.ViewID
        );
    }

    [PunRPC]
    void RPC_UsePlunger(int ownerViewID)
    {
        PhotonView ownerPV = PhotonView.Find(ownerViewID);
        if (ownerPV == null) return;

        PlungerSkill skill = ownerPV.GetComponent<PlungerSkill>();
        if (skill == null) return;

        GameObject proj = Instantiate(
            skill.plungerAttackPrefab,
            skill.handPoint.position,
            skill.handPoint.rotation
        );

        Vector3 fireDir = skill.handPoint.forward;

        proj.GetComponent<PlungerProjectile>()
            .Init(ownerPV, skill.pullForce, fireDir);

    }

    public void PlayShootSFX()
    {
        if (audioSource != null && shootSFX != null)
            audioSource.PlayOneShot(shootSFX);
    }
    public void PlayHitSFX()
    {
        if (audioSource != null && hitSFX != null)
            audioSource.PlayOneShot(hitSFX);
    }

    [PunRPC]
    void RPC_PlayShootSFX()
    {
        PlayShootSFX();
    }

    [PunRPC]
    public void RPC_PlayHitSFX()
    {
        PlayHitSFX();
    }


}
