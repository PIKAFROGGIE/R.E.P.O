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

        lastPosition = transform.position;
        lastRotation = transform.rotation;
    }

    void Update()
    {
        if (isSpectator) return;

        // Boss 正在判定时，检测玩家是否移动
        if (boss != null && boss.isDetecting)
        {
            CheckMovement();
        }
    }

    //关键：在 LateUpdate 里记录“这一帧最终位置”
    void LateUpdate()
    {
        if (isSpectator) return;

        lastPosition = transform.position;
        lastRotation = transform.rotation;
    }

    void CheckMovement()
    {
        // ===== 位移检测（忽略 Y，避免跳跃/下落误判）=====
        Vector3 delta = transform.position - lastPosition;
        delta.y = 0f;
        float moveDelta = delta.magnitude;

        // ===== 旋转检测 =====
        float rotDelta = Quaternion.Angle(transform.rotation, lastRotation);

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

    // ======================
    // 终点完成 → 观战
    // ======================
    public void ReachFinish()
    {
        if (isSpectator) return;
        photonView.RPC(nameof(RPC_Finish), RpcTarget.All);
    }

    [PunRPC]
    void RPC_Finish()
    {
        if (isSpectator) return;

        isSpectator = true;

        if (controller != null)
            controller.enabled = false;

        Debug.Log($"{photonView.Owner.NickName} finished and is now spectating");

        // TODO：
        // 切换观战相机
        // 显示完成 UI
    }

    // ======================
    // 时间到 → 失败 → 观战
    // ======================
    public void OnTimeUp()
    {
        if (isSpectator) return;
        photonView.RPC(nameof(RPC_TimeFailed), RpcTarget.All);
    }

    [PunRPC]
    void RPC_TimeFailed()
    {
        if (isSpectator) return;

        isSpectator = true;

        if (controller != null)
            controller.enabled = false;

        Debug.Log($"{photonView.Owner.NickName} failed: Time Up");

        // TODO：
        // UI 显示失败
        // 切换观战相机
    }
}
