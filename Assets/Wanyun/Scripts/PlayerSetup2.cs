using UnityEngine;
using Photon.Pun;

public class PlayerSetup2 : MonoBehaviourPunCallbacks
{
    [Header("Model Parts")]
    public GameObject[] FPS_Hands_ChildGameobjects;
    public GameObject[] Soldier_ChildGameobjects;

    [Header("UI / Camera")]
    public GameObject playerUIPrefab;
    public Camera FPSCamera;

    private PlayerController playerController;
    private Animator animator;
    private CharacterController characterController;
    private Rigidbody rb;

    void Start()
    {
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
        characterController = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();

        if (photonView.IsMine)
        {
            SetupLocalPlayer();
        }
        else
        {
            SetupRemotePlayer();
        }
    }

    // =========================
    // 本地玩家
    // =========================
    void SetupLocalPlayer()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // 隐藏第三人称模型（只显示手）
        foreach (GameObject go in Soldier_ChildGameobjects)
            go.SetActive(false);

        // 启用本地控制
        if (playerController != null)
            playerController.enabled = true;

        // 启用 CharacterController（本地才算物理）
        if (characterController != null)
            characterController.enabled = true;

        // Rigidbody 只作为占位，防止双物理
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // Camera 只给本地玩家
        if (FPSCamera != null)
            FPSCamera.enabled = true;

        // UI 只生成一次（本地）
        if (playerUIPrefab != null)
            Instantiate(playerUIPrefab);

        // Animator 设置
        if (animator != null)
        {
            animator.applyRootMotion = false;
            animator.SetBool("IsSoldier", false);
        }
    }

    // =========================
    // 远端玩家
    // =========================
    void SetupRemotePlayer()
    {
        // 显示第三人称模型
        foreach (GameObject go in Soldier_ChildGameobjects)
            go.SetActive(true);

        // 禁用输入 & 控制
        if (playerController != null)
            playerController.enabled = false;

        // ❌ 远端玩家不算物理
        if (characterController != null)
            characterController.enabled = false;

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // 关闭所有 Camera
        foreach (Camera cam in GetComponentsInChildren<Camera>())
            cam.enabled = false;

        // Animator 不允许写位移
        if (animator != null)
            animator.applyRootMotion = false;
    }
}
