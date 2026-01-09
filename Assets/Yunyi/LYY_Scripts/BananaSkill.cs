using UnityEngine;
using Photon.Pun;

public class BananaSkill : MonoBehaviourPun
{
    public GameObject bananaProjectilePrefab;
    public Transform firePoint;

    public float flyDistance = 4f;

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip throwSFX;
    public AudioClip bananaSFX;

    public void Activate()
    {
        if (!photonView.IsMine) return;

        Vector3 firePos = firePoint.position;
        Vector3 fireDir = firePoint.forward;

        photonView.RPC(
        nameof(RPC_PlayThrowSFX),
        RpcTarget.All
        );

        photonView.RPC(
            nameof(RPC_UseBanana),
            RpcTarget.All,
            photonView.ViewID,
            firePos,
            fireDir
        );
    }

    [PunRPC]
    void RPC_UseBanana(int ownerViewID, Vector3 firePos, Vector3 fireDir)
    {
        PhotonView ownerPV = PhotonView.Find(ownerViewID);
        if (ownerPV == null) return;

        BananaSkill skill = ownerPV.GetComponent<BananaSkill>();
        if (skill == null) return;

        GameObject proj = Instantiate(
            skill.bananaProjectilePrefab,
            firePos,
            Quaternion.LookRotation(fireDir)
        );

        proj.GetComponent<BananaProjectile>()
            .Init(ownerPV, fireDir, skill.flyDistance);
    }

    void PlayThrowSFX()
    {
        if (audioSource != null && throwSFX != null)
            audioSource.PlayOneShot(throwSFX);
    }

    void PlayBananaHitSFX()
    {
        if (audioSource != null && throwSFX != null)
            audioSource.PlayOneShot(bananaSFX);
    }

    [PunRPC]
    void RPC_PlayThrowSFX()
    {
        PlayThrowSFX();
    }

    [PunRPC]
    public void RPC_PlayBananaHitSFX()
    {
        PlayBananaHitSFX();
    }

}
