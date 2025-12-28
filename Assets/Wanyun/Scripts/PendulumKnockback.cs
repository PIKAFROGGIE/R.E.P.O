using UnityEngine;
using Photon.Pun;
using System.Collections;

public class PendulumKnockback : MonoBehaviour
{
    [Header("Knockback Settings")]
    public float knockbackForce = 18f;       // 横向力度（要比之前大）
    public float knockbackDuration = 0.35f;  // 受力时间
    public float upwardForce = 4f;           // 明显向上
    public float stunDuration = 0.4f;        // 控制锁定时间

    private bool canHit = true;

    private void OnCollisionEnter(Collision collision)
    {
        if (!canHit) return;
        if (!collision.collider.CompareTag("Player")) return;

        PlayerController player = collision.collider.GetComponent<PlayerController>();
        if (player == null) return;

        PhotonView pv = player.GetComponent<PhotonView>();
        if (pv != null && !pv.IsMine) return;

        CharacterController cc = player.CC;
        if (cc == null) return;

        // 计算击飞方向（摆锤 → 玩家）
        Vector3 dir = (collision.transform.position - transform.position);
        dir.y = 0f;
        dir.Normalize();

        StartCoroutine(KnockbackRoutine(player, cc, dir));
        StartCoroutine(HitCooldown());
    }

    IEnumerator KnockbackRoutine(PlayerController player, CharacterController cc, Vector3 dir)
    {
        canHit = false;

        // 🔒 锁定玩家输入（关键）
        player.SetStunned(true);

        float timer = 0f;

        while (timer < knockbackDuration)
        {
            Vector3 move =
                dir * knockbackForce +
                Vector3.up * upwardForce;

            cc.Move(move * Time.deltaTime);

            timer += Time.deltaTime;
            yield return null;
        }

        // 给一点额外滞空感
        yield return new WaitForSeconds(0.1f);

        player.SetStunned(false);
    }

    IEnumerator HitCooldown()
    {
        yield return new WaitForSeconds(0.6f);
        canHit = true;
    }
}
