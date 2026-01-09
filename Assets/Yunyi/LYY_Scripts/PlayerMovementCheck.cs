using UnityEngine;
using Photon.Pun;

public class PlayerMovementCheck : MonoBehaviourPun
{
    [Header("References")]
    public BossController boss;
    public FallRespawn fallRespawn;
    public CharacterController controller;

    [Header("Boss Detection Threshold")]
    public float moveThreshold = 0.02f;
    public float rotationThreshold = 2f;

    // 上一帧记录
    private Vector3 lastPosition;
    private Quaternion lastRotation;

    // 状态
    private bool isSpectator = false;

    void Start()
    {
        // 只在本地玩家身上检测
        if (!photonView.IsMine)
        {
            enabled = false;
            return;
        }

        // ⭐ 关键：运行时查找 BossController
        if (boss == null)
        {
            boss = FindObjectOfType<BossController>();

            if (boss == null)
            {
                Debug.LogError("BossController not found in scene!");
            }
        }

        lastPosition = transform.position;
        lastRotation = transform.rotation;
    }

    void Update()
    {
        if (isSpectator) return;

        // ⭐ 关键：先记录“上一帧最终位置”
        Vector3 prevPos = lastPosition;
        Quaternion prevRot = lastRotation;

        // 更新 lastPosition，供下一帧使用
        lastPosition = transform.position;
        lastRotation = transform.rotation;

        // Boss 正在判定时，用 prev 对比 current
        if (boss != null && boss.isDetecting)
        {
            CheckMovement(prevPos, prevRot);
        }
    }

    void CheckMovement(Vector3 prevPos, Quaternion prevRot)
    {
        Vector3 delta = transform.position - prevPos;
        delta.y = 0f;

        float moveDelta = delta.magnitude;
        float rotDelta = Quaternion.Angle(transform.rotation, prevRot);

        if (moveDelta > moveThreshold || rotDelta > rotationThreshold)
        {
            TriggerRespawn();
        }
    }

    public void ForceViolation()
    {
        if (boss != null && boss.isDetecting)
        {
            TriggerRespawn(); // 或 Die / Eliminate
        }
    }

    [PunRPC]
    public void RPC_ForceViolation()
    {
        if (!photonView.IsMine) return; // 只处理自己的违规
        ForceViolation();
    }

    void TriggerRespawn()
    {
        if (fallRespawn != null)
        {
            fallRespawn.ForceRespawn();
        }
    }
}
