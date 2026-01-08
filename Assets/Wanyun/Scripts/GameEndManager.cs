using UnityEngine;
using Photon.Pun;
using System.Collections;

public class GameEndManager : MonoBehaviourPunCallbacks
{
    public static GameEndManager Instance;

    [Header("Mode")]
    [Tooltip("勾选：Photon 网络模式；不勾选：本地模式")]
    public bool usePhotonSync = false;

    [Header("End Countdown Settings")]
    public float endCountdownDuration = 20f;

    [Header("Local Win Barrier (Local Only)")]
    public GameObject localWinBarrier;
    public float localBarrierDelay = 5f;

    private double endTime;
    private bool countdownStarted = false;
    private bool gameEnded = false;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (localWinBarrier != null)
            localWinBarrier.SetActive(false);
    }

    void Update()
    {
        if (!countdownStarted || gameEnded) return;

        double currentTime = usePhotonSync ? PhotonNetwork.Time : Time.time;
        double timeLeft = endTime - currentTime;

        PlayerUIManager ui = GetLocalPlayerUI();
        if (ui != null)
        {
            if (timeLeft > 0)
            {
                ui.UpdateCountdown(Mathf.CeilToInt((float)timeLeft));
            }
            else
            {
                EndGame(ui);
            }
        }
    }


    public void OnPlayerReachedFinish(PlayerUIManager ui)
    {
        if (countdownStarted) return;

        if (ui != null)
            ui.ShowWinText();

        EnableLocalWinBarrier();

        if (!usePhotonSync)
        {
            endTime = Time.time + endCountdownDuration;
            countdownStarted = true;
            return;
        }

        if (!PhotonNetwork.IsMasterClient) return;

        endTime = PhotonNetwork.Time + endCountdownDuration;
        photonView.RPC(nameof(RPC_StartEndCountdown), RpcTarget.All, endTime);
    }

    [PunRPC]
    void RPC_StartEndCountdown(double syncedEndTime)
    {
        endTime = syncedEndTime;
        countdownStarted = true;
    }

    // =========================
    // Local Barrier
    // =========================

    void EnableLocalWinBarrier()
    {
        if (localWinBarrier == null) return;

        StopCoroutine(nameof(EnableLocalBarrierAfterDelay));
        StartCoroutine(EnableLocalBarrierAfterDelay());
    }

    IEnumerator EnableLocalBarrierAfterDelay()
    {
        yield return new WaitForSeconds(localBarrierDelay);

        if (localWinBarrier != null)
            localWinBarrier.SetActive(true);
    }

    // =========================
    // End Game
    // =========================

    void EndGame(PlayerUIManager ui)
    {
        if (gameEnded) return;
        gameEnded = true;

        if (ui != null)
            ui.ShowGameOver();

        Debug.Log("Game Ended");

    }


    PlayerUIManager GetLocalPlayerUI()
    {
        if (PhotonNetwork.LocalPlayer?.TagObject == null)
            return null;

        GameObject player = PhotonNetwork.LocalPlayer.TagObject as GameObject;
        if (player == null) return null;

        return player.GetComponent<PlayerUIManager>();
    }
}
