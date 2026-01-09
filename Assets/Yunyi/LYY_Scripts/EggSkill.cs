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

        Vector3 firePos = handPoint.position;
        Vector3 fireDir = handPoint.forward;

        photonView.RPC(
            nameof(RPC_PlayEggShootSFX),
            RpcTarget.All
        );

        photonView.RPC(
            nameof(RPC_UseEgg),
            RpcTarget.All,
            photonView.ViewID,
            firePos,
            fireDir
        );
    }

    [PunRPC]
    void RPC_UseEgg(int ownerViewID, Vector3 firePos, Vector3 fireDir)
    {
        PhotonView ownerPV = PhotonView.Find(ownerViewID);
        if (ownerPV == null) return;

        EggSkill skill = ownerPV.GetComponent<EggSkill>();
        if (skill == null) return;

        GameObject proj = Instantiate(
            skill.eggProjectilePrefab,
            firePos,
            Quaternion.LookRotation(fireDir)
        );

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
