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

    private Vector3 lastPosition;
    private Quaternion lastRotation;

    private bool isSpectator = false;

    void Start()
    {
        if (!photonView.IsMine)
        {
            enabled = false;
            return;
        }

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

        Vector3 prevPos = lastPosition;
        Quaternion prevRot = lastRotation;

        lastPosition = transform.position;
        lastRotation = transform.rotation;

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
            TriggerRespawn();
        }
    }

    [PunRPC]
    public void RPC_ForceViolation()
    {
        if (!photonView.IsMine) return;
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
