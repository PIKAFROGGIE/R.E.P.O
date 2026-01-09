using UnityEngine;
using Photon.Pun;

public class ThunderSkill : MonoBehaviourPun
{
    [Header("Thunder Settings")]
    public float radius = 5f;
    public float force = 12f;

    [Header("VFX")]
    public GameObject thunderVFXPrefab;
    public float vfxDuration = 0.6f;

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip skillSFX;

    public void Activate()
    {
        if (!photonView.IsMine) return;

        photonView.RPC(nameof(RPC_Thunder), RpcTarget.All, transform.position);
    }

    [PunRPC]
    void RPC_Thunder(Vector3 center)
    {
        Debug.Log("⚡ Thunder triggered");

        photonView.RPC(
        nameof(RPC_PlaySkillSFX),
        RpcTarget.All
        );

        if (thunderVFXPrefab != null)
        {
            GameObject vfx = Instantiate(thunderVFXPrefab, center, Quaternion.identity);

            float diameter = radius * 2f;
            vfx.transform.localScale = new Vector3(diameter, diameter, diameter);

            Destroy(vfx, vfxDuration);
        }

        Collider[] hits = Physics.OverlapSphere(center, radius);

        foreach (Collider hit in hits)
        {
            PhotonView targetPV = hit.GetComponentInParent<PhotonView>();
            PlayerKnockback1 knockback = hit.GetComponentInParent<PlayerKnockback1>();

            if (targetPV == null || knockback == null) continue;
            if (targetPV == photonView) continue;

            Vector3 dir = targetPV.transform.position - center;

            targetPV.RPC(
                nameof(PlayerKnockback1.RPC_ApplyKnockback),
                RpcTarget.All,
                dir,
                force
            );
        }
    }
    public void PlaySkillSFX()
    {
        if (audioSource != null && skillSFX != null)
            audioSource.PlayOneShot(skillSFX);
    }

    [PunRPC]
    void RPC_PlaySkillSFX()
    {
        PlaySkillSFX();
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
#endif
}
