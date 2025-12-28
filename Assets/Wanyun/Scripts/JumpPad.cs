using UnityEngine;
using Photon.Pun;
using System.Collections;

public class JumpPad : MonoBehaviour
{
    [Header("Jump Pad Settings")]
    public float launchVelocity = 12f;   // 初始向上速度
    public float launchDuration = 0.2f;  // 推力持续时间

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;

        PhotonView pv = player.GetComponent<PhotonView>();
        if (pv != null && !pv.IsMine) return;

        StartCoroutine(LaunchPlayer(player));
    }

    IEnumerator LaunchPlayer(PlayerController player)
    {
        CharacterController cc = player.CC;
        if (cc == null) yield break;

        float timer = 0f;

        // 发射阶段（模拟“弹”）
        while (timer < launchDuration)
        {
            Vector3 velocity = Vector3.up * launchVelocity;
            cc.Move(velocity * Time.deltaTime);

            timer += Time.deltaTime;
            yield return null;
        }
    }
}
