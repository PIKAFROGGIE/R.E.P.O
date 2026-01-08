using UnityEngine;
using Photon.Pun;
using Unity.Burst.CompilerServices;

public class ThunderSkill : MonoBehaviourPun
{
    [Header("Thunder Settings")]
    public float radius = 5f;
    public float force = 12f;

    [Header("VFX")]
    public GameObject thunderVFXPrefab;
    public float vfxDuration = 0.6f; // 光罩存在时间

    public void Activate()
    {
        if (!photonView.IsMine) return;

        photonView.RPC(nameof(RPC_Thunder), RpcTarget.All, transform.position);
    }

    [PunRPC]
    void RPC_Thunder(Vector3 center)
    {
        Debug.Log("⚡ Thunder triggered");

        Collider[] hits = Physics.OverlapSphere(center, radius);

        foreach (Collider hit in hits)
        {
            PhotonView targetPV = hit.GetComponentInParent<PhotonView>();
            PlayerKnockback1 knockback = hit.GetComponentInParent<PlayerKnockback1>();

            if (targetPV == null || knockback == null) continue;
            if (targetPV == photonView) continue; // 不影响自己

            // ===== 1. 震飞（你原本就 OK 的逻辑）=====
            Vector3 dir = targetPV.transform.position - center;

            targetPV.RPC(
                nameof(PlayerKnockback1.RPC_ApplyKnockback),
                RpcTarget.All,
                dir,
                force
            );

            // ===== 2. 规则级违规（只针对被击中的玩家）=====
            PlayerMovementCheck movementCheck =
                targetPV.GetComponent<PlayerMovementCheck>();

            if (movementCheck != null)
            {
                targetPV.RPC(
                    nameof(PlayerMovementCheck.RPC_ForceViolation),
                    RpcTarget.All
                );
            }
        }
    }


#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
#endif
}
