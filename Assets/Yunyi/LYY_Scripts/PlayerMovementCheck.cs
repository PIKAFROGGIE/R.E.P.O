using UnityEngine;
using Photon.Pun;

public class PlayerMovementCheck : MonoBehaviourPun
{
    [Header("References")]
    public BossController boss;
    public FallRespawn fallRespawn;
    public CharacterController controller;

    [Header("Boss Detection")]
    public float moveThreshold = 0.02f;
    public float rotationThreshold = 2f;

    Vector3 lastPosition;
    Quaternion lastRotation;

    bool isSpectator = false;

    void Start()
    {
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

        // ===== Boss 判定 =====
        if (boss != null && boss.isDetecting)
        {
            CheckMovement();
        }

        lastPosition = transform.position;
        lastRotation = transform.rotation;
    }

    void CheckMovement()
    {
        float moveDelta = Vector3.Distance(transform.position, lastPosition);
        float rotDelta = Quaternion.Angle(transform.rotation, lastRotation);

        if (moveDelta > moveThreshold || rotDelta > rotationThreshold)
        {
            TriggerRespawn();
        }
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
        isSpectator = true;

        if (controller != null)
            controller.enabled = false;

        Debug.Log($"{photonView.Owner.NickName} finished and is now spectating");

        // TODO：
        // 切换观战相机
        // 显示完成 UI
    }
}
