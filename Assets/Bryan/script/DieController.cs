using UnityEngine;
using Photon.Pun;
using System.Collections;

public class DieController : MonoBehaviour
{
    [Header("Spectator Settings")]
    [Tooltip("The script that handles flying/spectating (drag SpectatorMovement here)")]
    public MonoBehaviour spectatorScript;

    [Tooltip("Scripts to turn off when dead (e.g., PlayerMovement, Shooting)")]
    public MonoBehaviour[] scriptsToDisable;

    [Tooltip("GameObjects to hide when dead (e.g., the Mesh/Body)")]
    public GameObject[] visualRoots;

    [Header("Fade")]
    public CanvasGroup fadeCanvas;
    public float fadeDuration = 0.5f;

    PhotonView pv;
    CharacterController cc;
    Collider[] myColliders;

    bool isDead = false;

    // Helper to check ownership regardless of Online/Offline status
    bool IsLocalPlayer
    {
        get
        {
            // If offline, we are always the local player
            if (!PhotonNetwork.IsConnected) return true;
            // If online, check PhotonView
            return pv.IsMine;
        }
    }

    void Start()
    {
        pv = GetComponent<PhotonView>();
        cc = GetComponent<CharacterController>();
        myColliders = GetComponents<Collider>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!IsLocalPlayer) return; // Changed from pv.IsMine
        if (isDead) return;

        if (other.CompareTag("KillZone"))
        {
            StartCoroutine(DieSequence());
        }
    }

    public void ForceKill()
    {
        if (!IsLocalPlayer || isDead) return;
        StartCoroutine(DieSequence());
    }

    IEnumerator DieSequence()
    {
        isDead = true;

        // 1. Fade Out
        yield return StartCoroutine(Fade(1f));

        // 2. Hide Visuals / Disable Physics
        if (PhotonNetwork.IsConnected && pv != null)
        {
            // ONLINE: Tell everyone else to hide our body
            pv.RPC("RPC_BecomeSpectator", RpcTarget.AllBuffered);
        }
        else
        {
            // OFFLINE: Just do it locally immediately
            RPC_BecomeSpectator();
        }

        // 3. Switch Controls (Local only)
        EnableSpectatorControls();

        // 4. Fade In
        yield return StartCoroutine(Fade(0f));
    }

    [PunRPC]
    void RPC_BecomeSpectator()
    {
        // 1. Hide Visuals
        foreach (var obj in visualRoots)
        {
            if (obj != null) obj.SetActive(false);
        }

        // 2. Disable Colliders
        if (cc != null) cc.enabled = false;
        foreach (var col in myColliders)
        {
            col.enabled = false;
        }
    }

    void EnableSpectatorControls()
    {
        // Disable walker scripts
        foreach (var script in scriptsToDisable)
        {
            if (script != null) script.enabled = false;
        }

        // Enable flyer script
        if (spectatorScript != null)
        {
            spectatorScript.enabled = true;
        }
    }

    IEnumerator Fade(float target)
    {
        if (fadeCanvas == null) yield break;

        float start = fadeCanvas.alpha;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadeCanvas.alpha = Mathf.Lerp(start, target, t / fadeDuration);
            yield return null;
        }
        fadeCanvas.alpha = target;
    }
}