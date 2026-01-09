using UnityEngine;
using Photon.Pun;
using System.Collections;

public class BananaPeel : MonoBehaviourPun
{
    public float slipDuration = 0.6f;
    public float slipForce = 6f;

    bool triggered = false;

    [Header("Sound")]
    public AudioClip slipSFX;

    void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        PlayerController pc = other.GetComponentInParent<PlayerController>();
        if (pc == null) return;

        PhotonView targetPV = pc.GetComponent<PhotonView>();
        PlayerKnockback1 knockback = pc.GetComponent<PlayerKnockback1>();
        if (targetPV == null || knockback == null) return;

        triggered = true;

        // ⭐ 只有被踩玩家自己发 RPC
        if (targetPV.IsMine)
        {
            BananaSkill skill = targetPV.GetComponent<BananaSkill>();
            if (skill != null)
            {
                skill.photonView.RPC(
                    nameof(BananaSkill.RPC_PlayBananaHitSFX),
                    RpcTarget.All
                );
            }

            // ✅ 临时锁控（在玩家身上开协程，不会被香蕉皮销毁打断）
            targetPV.RPC(nameof(PlayerController.RPC_TempControlLock), RpcTarget.All, slipDuration);

            // 强制滑行
            Vector3 slipDir = targetPV.transform.forward;
            targetPV.RPC(
                nameof(PlayerKnockback1.RPC_ApplyKnockback),
                RpcTarget.All,
                slipDir,
                slipForce
            );

            // ✅ 触发后立刻清理香蕉皮（现在不会影响解锁了）
            targetPV.RPC(
                nameof(PlayerController.RPC_DestroyAllBananaPeels),
                RpcTarget.All
            );
        }

    }


    IEnumerator Recover(PhotonView targetPV)
    {
        yield return new WaitForSeconds(slipDuration);

        if (targetPV != null)
        {
            targetPV.RPC(nameof(PlayerController.RPC_RemoveControlLock), RpcTarget.All);
        }
    }

    void PlaySlipSFX()
    {
        if (slipSFX != null)
        {
            AudioSource.PlayClipAtPoint(
                slipSFX,
                Camera.main.transform.position
            );

        }
    }

}
