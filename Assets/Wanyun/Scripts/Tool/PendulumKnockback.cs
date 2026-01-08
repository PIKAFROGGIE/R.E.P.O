using UnityEngine;
using Photon.Pun;
using System.Collections;

public class PendulumKnockback : MonoBehaviour
{
    public float force = 12f;
    public float upwardForce = 6f;

    void OnTriggerEnter(Collider other)
    {
        PlayerKnockback player = other.GetComponentInParent<PlayerKnockback>();
        if (player == null) return;

        // 从机关指向玩家的方向
        Vector3 dir = (other.transform.position - transform.position);
        dir.y = 0f; // 不要往地面推

        player.ApplyKnockback(dir, force, upwardForce);
        AudioManager.Instance.PlaySFX(SFXType.Hit);
    }
}
