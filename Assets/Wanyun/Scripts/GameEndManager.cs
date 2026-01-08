using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

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

    [Header("UI")]
    public Text timeText;
    public TMP_Text winText;
    public float winTextShowDuration = 3f;

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

        if (localWinBarrier != null)
            localWinBarrier.SetActive(false); 
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

    public void OnPlayerReachedFinish()
    {
        if (countdownStarted) return;

        //本地 Barrier：只对当前玩家生效
        EnableLocalWinBarrier();
        ShowWinText();

        //本地模式
        if (!usePhotonSync)
        {
            endTime = Time.time + endCountdownDuration;
            countdownStarted = true;
            return;
        }

        //网络模式：只有 MasterClient 决定倒计时
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

        StopCoroutine(nameof(HideWinTextAfterDelay));
        StartCoroutine(HideWinTextAfterDelay());
    }

    IEnumerator HideWinTextAfterDelay()
    {
        yield return new WaitForSeconds(winTextShowDuration);

        if (winText != null)
            winText.gameObject.SetActive(false);
    }

    public void EnableLocalWinBarrier()
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



    void EndGame()
    {
        gameEnded = true;

        if (timeText != null)
            timeText.text = "GAME OVER";

        Debug.Log("Game Ended");

        // 👉 这里可以继续扩展：
        // - 禁止玩家操作
        // - 打开结算界面
        // - 延迟切换场景
    }
}

