using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class GameEndManager : MonoBehaviourPunCallbacks
{
    public static GameEndManager Instance;

    [Header("Mode")]
    [Tooltip("勾选：Photon 网络模式；不勾选：本地模式")]
    public bool usePhotonSync = false;

    [Header("End Countdown Settings")]
    public float endCountdownDuration = 20f; // ⏱ 可自定义（秒）

    [Header("UI")]
    public Text timeText; // Text (Legacy)
    public Text winText;   // 显示 "YOU WIN!"

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

        if (winText != null)
            winText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!countdownStarted || gameEnded) return;

        double currentTime = usePhotonSync ? PhotonNetwork.Time : Time.time;
        double timeLeft = endTime - currentTime;

        if (timeLeft > 0)
        {
            timeText.text = $"{Mathf.CeilToInt((float)timeLeft)}";
        }
        else
        {
            EndGame();
        }
    }

    /// <summary>
    /// 被 FinishLineTrigger 调用
    /// </summary>
    public void OnPlayerReachedFinish()
    {
        if (countdownStarted) return;

        // 🔹 本地模式：直接开始
        if (!usePhotonSync)
        {
            endTime = Time.time + endCountdownDuration;
            countdownStarted = true;
            return;
        }

        // 🔹 网络模式：只有 MasterClient 决定
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

    public void ShowWinText()
    {
        if (winText == null) return;

        winText.text = "YOU WIN!";
        winText.gameObject.SetActive(true);
    }

    void EndGame()
    {
        gameEnded = true;
        timeText.text = "GAME OVER";

        Debug.Log("Game Ended");

        // 👉 你可以在这里做：
        // - 禁止玩家操作
        // - 打开结算界面
        // - 延迟切换场景
    }
}
