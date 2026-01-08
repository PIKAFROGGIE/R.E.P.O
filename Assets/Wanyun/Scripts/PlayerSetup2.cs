using UnityEngine;
using Photon.Pun;
using Cinemachine;

public class PlayerSetup2 : MonoBehaviourPunCallbacks
{
    [Header("UI")]
    public GameObject playerUIPrefab;

    [Header("Cinemachine (FreeLook)")]
    public CinemachineVirtualCameraBase vcam;   // ✅ 兼容 FreeLook / VirtualCamera
    public Transform followTarget;              // FreeLook 的 Follow
    public Transform lookAtTarget;              // FreeLook 的 LookAt

    private PlayerController playerController;
    private CharacterController characterController;
    private Rigidbody rb;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
        characterController = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();

        if (vcam == null)
            vcam = GetComponentInChildren<CinemachineVirtualCameraBase>(true);

        if (photonView.IsMine) SetupLocalPlayer();
        else SetupRemotePlayer();
    }

    void SetupLocalPlayer()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // 本地允许控制
        if (playerController != null) playerController.enabled = true;
        if (characterController != null) characterController.enabled = true;

        // 防双物理
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // ✅ 本地启用 FreeLook（提高优先级）
        if (vcam != null)
        {
            vcam.Priority = 20;
            vcam.gameObject.SetActive(true);

            // FreeLook 需要 Follow/LookAt（可拖，也可自动设）
            if (vcam is CinemachineFreeLook freeLook)
            {
                if (followTarget != null) freeLook.Follow = followTarget;
                if (lookAtTarget != null) freeLook.LookAt = lookAtTarget;
            }
        }

        if (playerUIPrefab != null) Instantiate(playerUIPrefab);

        if (animator != null) animator.applyRootMotion = false;
    }

    void SetupRemotePlayer()
    {
        // 远端禁用控制
        if (playerController != null) playerController.enabled = false;
        if (characterController != null) characterController.enabled = false;

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // ✅ 远端 FreeLook 必须失效（否则抢镜头）
        if (vcam != null)
        {
            vcam.Priority = 0;
            vcam.gameObject.SetActive(false);
        }

        if (animator != null) animator.applyRootMotion = false;
    }
}
