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

    // ⚡ 被 Thunder 调用的 RPC
    [PunRPC]
    public void RPC_ApplyKnockback(Vector3 direction, float force)
    {
        if (isKnockbacking) return;
        StartCoroutine(KnockbackRoutine(direction, force));
    }

    IEnumerator KnockbackRoutine(Vector3 direction, float force)
    {
        isKnockbacking = true;

        float timer = 0f;
        direction.y = 0f;
        direction.Normalize();

        while (timer < knockDuration)
        {
            controller.Move(direction * force * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }

        isKnockbacking = false;
    }
}
