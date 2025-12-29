using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class GameEndManager : MonoBehaviourPunCallbacks
{
    public static GameEndManager Instance;

    [Header("End Countdown Settings")]
    public float endCountdownDuration = 20f; // ⏱ 可自定义（秒）

    [Header("UI")]
    public Text timeText; // 普通 Text (Legacy)

    private double endTime;
    private bool countdownStarted = false;
    private bool gameEnded = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Update()
    {
        if (!countdownStarted || gameEnded) return;

        double timeLeft = endTime - PhotonNetwork.Time;

        if (timeLeft > 0)
        {
            timeText.text = $"Game Ends In: {Mathf.CeilToInt((float)timeLeft)}";
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
        if (!PhotonNetwork.IsMasterClient) return;

        if (!countdownStarted)
        {
            endTime = PhotonNetwork.Time + endCountdownDuration;
            photonView.RPC(nameof(RPC_StartEndCountdown), RpcTarget.All, endTime);
        }
    }

    [PunRPC]
    void RPC_StartEndCountdown(double syncedEndTime)
    {
        endTime = syncedEndTime;
        countdownStarted = true;
    }

    void EndGame()
    {
        gameEnded = true;
        timeText.text = "GAME OVER";

        // 👉 这里你可以做：
        // - 禁止玩家移动
        // - 显示结算 UI
        // - 切换场景
        Debug.Log("Game Ended");
    }
}
