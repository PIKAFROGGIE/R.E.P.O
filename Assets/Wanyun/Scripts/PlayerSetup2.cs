using UnityEngine;
using Photon.Pun;
using Cinemachine;

public class PlayerSetup2 : MonoBehaviourPunCallbacks
{
    [Header("UI")]
    public GameObject playerUIPrefab;

    [Header("Cinemachine")]
    public CinemachineVirtualCameraBase vcam;
    public Transform followTarget;
    public Transform lookAtTarget; 
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

        if (playerController != null) playerController.enabled = true;
        if (characterController != null) characterController.enabled = true;

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        if (vcam != null)
        {
            vcam.Priority = 20;
            vcam.gameObject.SetActive(true);

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
        if (playerController != null) playerController.enabled = false;
        if (characterController != null) characterController.enabled = false;

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        if (vcam != null)
        {
            vcam.Priority = 0;
            vcam.gameObject.SetActive(false);
        }

        if (animator != null) animator.applyRootMotion = false;
    }
}
