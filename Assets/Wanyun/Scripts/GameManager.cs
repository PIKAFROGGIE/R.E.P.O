using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("Mode")]
    [Tooltip("勾选：使用 Photon 同步；不勾选：本地倒数")]
    public bool usePhotonSync = false;

    [Header("Countdown")]
    public float prepareTime = 5f;

    [Header("UI")]
    public CanvasGroup countdownCanvas;
    public Text countdownText;

    [Header("Barrier")]
    public GameObject startBarrier;

    private double startTime;
    private bool countdownStarted = false;
    private bool gameStarted = false;

    void Start()
    {
        startBarrier.SetActive(true);
        countdownCanvas.alpha = 1;

        // 👉 本地模式：直接开始倒数
        if (!usePhotonSync)
        {
            startTime = Time.time + prepareTime;
            countdownStarted = true;
        }
    }

    public override void OnJoinedRoom()
    {
        // 👉 如果没开 Photon，同步逻辑直接跳过
        if (!usePhotonSync) return;

        if (PhotonNetwork.IsMasterClient)
        {
            startTime = PhotonNetwork.Time + prepareTime;
            photonView.RPC(nameof(RPC_SetStartTime), RpcTarget.All, startTime);
        }
    }

    [PunRPC]
    void RPC_SetStartTime(double serverStartTime)
    {
        startTime = serverStartTime;
        countdownStarted = true;
    }

    void Update()
    {
        if (!countdownStarted || gameStarted) return;

        double currentTime = usePhotonSync ? PhotonNetwork.Time : Time.time;
        double timeLeft = startTime - currentTime;

        if (timeLeft > 3)
        {
            countdownText.text = "Ready?";
        }
        else if (timeLeft > 2)
        {
            countdownText.text = "3";
        }
        else if (timeLeft > 1)
        {
            countdownText.text = "2";
        }
        else if (timeLeft > 0)
        {
            countdownText.text = "1";
        }
        else
        {
            StartGame();
        }
    }

    void StartGame()
    {
        gameStarted = true;

        countdownText.text = "GO!";
        startBarrier.SetActive(false);

        StartCoroutine(HideUI());
    }

    IEnumerator HideUI()
    {
        yield return new WaitForSeconds(0.8f);

        float t = 0f;
        while (t < 0.5f)
        {
            t += Time.deltaTime;
            countdownCanvas.alpha = Mathf.Lerp(1, 0, t / 0.5f);
            yield return null;
        }

        countdownCanvas.alpha = 0;
    }
}
