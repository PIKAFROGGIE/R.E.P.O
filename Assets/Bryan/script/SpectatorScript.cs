using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;

public class PhotonSpectatorScript : MonoBehaviour
{
    [Header("Spectator Settings")]
    public Vector3 followOffset = new Vector3(0, 3, -5);
    public float smoothSpeed = 5f;

    [Header("Targeting")]
    public string playerTag = "Player";

    private List<GameObject> validTargets = new List<GameObject>();
    private int currentTargetIndex = 0;
    private Transform currentTarget;
    private DieController currentDieController;

    AudioListener listener;

    void Start()
    {
        listener = GetComponent<AudioListener>();
        listener.enabled = true;
        // Try to find someone immediately
        RefreshTargetList();
    }

    void Update()
    {
        // 1. Manual Switch (Tab)
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SwitchToNextPlayer();
        }

        // 2. Auto-Switch if we lost our target (Disconnected/Destroyed)
        if (currentTarget == null)
        {
            RefreshTargetList();
        }
        // 3. Auto-Switch if the person we are watching DIES
        else if (currentDieController != null && currentDieController.checkDead())
        {
            // CRITICAL FIX: 
            // We do NOT call SwitchToNextPlayer() here because that increases the index.
            // Instead, we 'forget' the current target and force a refresh. 
            // This makes the camera instantly snap to the first available survivor.
            currentTarget = null;
            RefreshTargetList();
        }
    }

    void LateUpdate()
    {
        if (currentTarget != null)
        {
            MoveCamera();
        }
    }

    void MoveCamera()
    {
        Vector3 desiredPosition = currentTarget.position + followOffset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.LookAt(currentTarget.position + Vector3.up);
    }

    // Only used when manually pressing TAB
    void SwitchToNextPlayer()
    {
        RefreshTargetList();

        if (validTargets.Count == 0) return;

        // Increment index to cycle through players
        currentTargetIndex = (currentTargetIndex + 1) % validTargets.Count;
        SetTarget(validTargets[currentTargetIndex]);
    }

    void RefreshTargetList()
    {
        validTargets.Clear();

        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag(playerTag);

        foreach (GameObject p in allPlayers)
        {
            PhotonView pView = p.GetComponent<PhotonView>();
            DieController dc = p.GetComponent<DieController>();

            // Filter: Must be Networked, Not Me, Have DieController, and Be Alive
            if (pView != null && !pView.IsMine && dc != null)
            {
                if (!dc.checkDead())
                {
                    validTargets.Add(p);
                }
            }
        }

        // If we currently have NO target (or we just set it to null because they died),
        // automatically grab the first survivor in the list.
        if (currentTarget == null && validTargets.Count > 0)
        {
            currentTargetIndex = 0;
            SetTarget(validTargets[0]);
        }
    }

    void SetTarget(GameObject targetObj)
    {
        currentTarget = targetObj.transform;
        currentDieController = targetObj.GetComponent<DieController>();
    }
}