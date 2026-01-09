using UnityEngine;
using Photon.Pun;
using System.Collections;

public class PlayerKnockback1 : MonoBehaviourPun
{
    public CharacterController controller;

    [Header("Knockback Settings")]
    public float knockDuration = 0.25f;

    bool isKnockbacking = false;

    void Awake()
    {
        if (controller == null)
            controller = GetComponent<CharacterController>();
    }

    // ⚡ 通用击退（Thunder / Banana 等）
    [PunRPC]
    public void RPC_ApplyKnockback(Vector3 direction, float force)
    {
        if (isKnockbacking) return;
        StartCoroutine(KnockbackRoutine(direction, force));
    }

    IEnumerator KnockbackRoutine(Vector3 direction, float force)
    {
        isKnockbacking = true;

        PlayerController pc = GetComponent<PlayerController>();
        if (pc != null)
        {
            // ⭐ 使用“加锁”
            PhotonView pv = GetComponent<PhotonView>();
            if (pv != null)
            {
                pv.RPC(nameof(PlayerController.RPC_AddControlLock), RpcTarget.All);
            }
        }

        float timer = 0f;
        direction.y = 0f;
        direction.Normalize();

        while (timer < knockDuration)
        {
            controller.Move(direction * force * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }

        if (pc != null)
        {
            // ⭐ 使用“解锁”
            PhotonView pv = GetComponent<PhotonView>();
            if (pv != null)
            {
                pv.RPC(nameof(PlayerController.RPC_RemoveControlLock), RpcTarget.All);
            }
        }

        isKnockbacking = false;
    }

    // 🪠 马桶塞拉人
    [PunRPC]
    public void RPC_PullToPosition(Vector3 targetPosition)
    {
        StartCoroutine(PullRoutine(targetPosition));
    }

    IEnumerator PullRoutine(Vector3 targetPosition)
    {
        isKnockbacking = true;

        PlayerController pc = GetComponent<PlayerController>();
        if (pc != null)
        {
            PhotonView pv = GetComponent<PhotonView>();
            if (pv != null)
            {
                pv.RPC(nameof(PlayerController.RPC_AddControlLock), RpcTarget.All);
            }
        }

        float t = 0f;
        float duration = 0.15f;

        Vector3 start = transform.position;

        while (t < duration)
        {
            Vector3 pos = Vector3.Lerp(start, targetPosition, t / duration);
            controller.enabled = false;
            transform.position = pos;
            controller.enabled = true;

            t += Time.deltaTime;
            yield return null;
        }

        controller.enabled = false;
        transform.position = targetPosition;
        controller.enabled = true;

        if (pc != null)
        {
            PhotonView pv = GetComponent<PhotonView>();
            if (pv != null)
            {
                pv.RPC(nameof(PlayerController.RPC_RemoveControlLock), RpcTarget.All);
            }
        }

        isKnockbacking = false;
    }
}
