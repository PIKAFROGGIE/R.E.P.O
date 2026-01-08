using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class ThunderSkill : MonoBehaviourPun
{
    [Header("Thunder Settings")]
    public float radius = 5f;
    public float force = 12f;

    [Header("VFX")]
    public GameObject thunderVFXPrefab;
    public float vfxDuration = 0.6f;
    public float vfxHeightOffset = 0.05f;   // 让光罩贴地
    public float vfxHeight = 0.4f;           // 压扁高度（视觉用）

    // ======================
    // 外部调用（由 PlayerItemHandler 调用）
    // ======================
    public void Activate()
    {
        if (!photonView.IsMine) return;

        photonView.RPC(
            nameof(RPC_Thunder),
            RpcTarget.All,
            transform.position
        );
    }

    // ======================
    // Thunder 主逻辑（所有客户端）
    // ======================
    [PunRPC]
    void RPC_Thunder(Vector3 center)
    {
        Debug.Log("⚡ Thunder triggered");

        // ========= 1. 特效 =========
        if (thunderVFXPrefab != null)
        {
            Vector3 vfxPos = center;
            vfxPos.y += vfxHeightOffset;

            GameObject vfx = Instantiate(thunderVFXPrefab, vfxPos, Quaternion.identity);

            float diameter = radius * 2f;
            vfx.transform.localScale = new Vector3(diameter, vfxHeight, diameter);

            Destroy(vfx, vfxDuration);
        }

        // ========= 2. 收集命中的玩家（去重） =========
        Collider[] hits = Physics.OverlapSphere(center, radius);

        HashSet<PhotonView> hitPlayers = new HashSet<PhotonView>();

        foreach (Collider hit in hits)
        {
            PhotonView pv = hit.GetComponentInParent<PhotonView>();
            if (pv != null)
            {
                hitPlayers.Add(pv);
            }
        }

        // ========= 3. 对每个玩家只处理一次 =========
        foreach (PhotonView targetPV in hitPlayers)
        {
            // 不作用到自己
            if (targetPV.Owner == photonView.Owner)
                continue;

            // ===== 击飞 =====
            PlayerKnockback1 knockback =
                targetPV.GetComponent<PlayerKnockback1>();

            if (knockback != null)
            {
                Vector3 dir = targetPV.transform.position - center;

                targetPV.RPC(
                    nameof(PlayerKnockback1.RPC_ApplyKnockback),
                    RpcTarget.All,
                    dir,
                    force
                );
            }

            // ===== 规则级违规 =====
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
