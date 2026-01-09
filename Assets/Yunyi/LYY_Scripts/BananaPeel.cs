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

            targetPV.RPC(nameof(PlayerController.RPC_TempControlLock), RpcTarget.All, slipDuration);

            Vector3 slipDir = targetPV.transform.forward;
            targetPV.RPC(
                nameof(PlayerKnockback1.RPC_ApplyKnockback),
                RpcTarget.All,
                slipDir,
                slipForce
            );

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
