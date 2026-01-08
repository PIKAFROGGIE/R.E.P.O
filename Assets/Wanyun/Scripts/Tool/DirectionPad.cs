using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DirectionPad : MonoBehaviour
{
    [Header("Pad Settings")]
    [Tooltip("带动速度（单位：米/秒）。方向由本物体 forward 决定。")]
    public float pushSpeed = 6f;

    [Tooltip("进入时是否平滑加到目标速度")]
    public bool smoothIn = true;

    [Tooltip("平滑时间（秒）")]
    public float smoothTime = 0.15f;

    // 记录在区域内的玩家
    private readonly HashSet<PlayerController> players = new HashSet<PlayerController>();

    // 给每个玩家单独做平滑
    private readonly Dictionary<PlayerController, float> currentFactor = new Dictionary<PlayerController, float>();

    private void Reset()
    {
        // 方便你一加脚本就能用：确保有 Collider 且默认 Trigger
        var col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var pc = other.GetComponent<PlayerController>();
        if (pc == null || pc.CC == null) return;

        var pv = pc.GetComponent<PhotonView>();
        if (pv != null && !pv.IsMine) return; // 只影响本地玩家

        players.Add(pc);
        if (!currentFactor.ContainsKey(pc)) currentFactor[pc] = 0f;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var pc = other.GetComponent<PlayerController>();
        if (pc == null) return;

        players.Remove(pc);
        currentFactor.Remove(pc);
    }

    private void LateUpdate()
    {
        if (players.Count == 0) return;

        // 方向由减速带自身朝向决定：
        // 正着放 = 推进（加速）；反着放 = 往回推（减速/倒推）；
        // 向右放 = 向右推；向左放 = 向左推。
        Vector3 dir = transform.forward;
        dir.y = 0f;
        dir.Normalize();

        // 逐个玩家施加“带动位移”
        foreach (var pc in players)
        {
            if (pc == null || pc.CC == null) continue;

            float factor = 1f;

            if (smoothIn)
            {
                // 平滑从 0 → 1，避免一下子被“拽飞”
                float cur = currentFactor.TryGetValue(pc, out var v) ? v : 0f;
                cur = Mathf.MoveTowards(cur, 1f, Time.deltaTime / Mathf.Max(0.001f, smoothTime));
                currentFactor[pc] = cur;
                factor = cur;
            }

            Vector3 externalMove = dir * (pushSpeed * factor) * Time.deltaTime;
            pc.CC.Move(externalMove);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // 画个方向箭头，方便你在场景里摆放方向
        Gizmos.color = Color.cyan;
        Vector3 p = transform.position;
        Vector3 f = transform.forward; f.y = 0;
        Gizmos.DrawLine(p, p + f.normalized * 2f);
        Gizmos.DrawSphere(p + f.normalized * 2f, 0.15f);
    }
#endif
}
