using UnityEngine;
using Photon.Pun;

public class EggSkill : MonoBehaviourPun
{
    public GameObject eggProjectilePrefab;
    public Transform handPoint;

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip shootSFX;
    public AudioClip hitSFX;

    public void Activate()
    {
        if (!photonView.IsMine) return;

        photonView.RPC(
        nameof(RPC_PlayEggShootSFX),
        RpcTarget.All
        );

        photonView.RPC(
            nameof(RPC_UseEgg),
            RpcTarget.All,
            photonView.ViewID
        );
    }

    [PunRPC]
    void RPC_UseEgg(int ownerViewID)
    {
        PhotonView ownerPV = PhotonView.Find(ownerViewID);
        if (ownerPV == null) return;

        EggSkill skill = ownerPV.GetComponent<EggSkill>();
        if (skill == null) return;

        GameObject proj = Instantiate(
            skill.eggProjectilePrefab,
            skill.handPoint.position,
            skill.handPoint.rotation
        );

        Vector3 fireDir = skill.handPoint.forward;

        proj.GetComponent<EggProjectile>()
            .Init(ownerPV, fireDir);
    }
    public void PlayEggShootSFX()
    {
        if (audioSource != null && shootSFX != null)
            audioSource.PlayOneShot(shootSFX);
    }
    public void PlayEggHitSFX()
    {
        if (audioSource != null && hitSFX != null)
            audioSource.PlayOneShot(hitSFX);
    }

    [PunRPC]
    void RPC_PlayEggShootSFX()
    {
        PlayEggShootSFX();
    }

    [PunRPC]
    public void RPC_PlayEggHitSFX()
    {
        PlayEggHitSFX();
    }
}
