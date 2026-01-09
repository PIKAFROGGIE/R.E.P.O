using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance;
    [Header("Mode")]
    public bool usePhotonSync = false;

    [Header("Countdown")]
    public float prepareTime = 5f;
    private bool countSoundPlayed = false;

    [Header("UI - Countdown")]
    public List<CanvasGroup> countdownCanvases = new List<CanvasGroup>();
    public List<Text> countdownTexts = new List<Text>();

    [Header("Barriers")]
    public List<GameObject> startBarriers = new List<GameObject>();

    private double startTime;
    private bool countdownStarted = false;
    private bool gameStarted = false;

    [Header("Boss")]
    public bool enableBoss = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        foreach (var barrier in startBarriers)
        {
            if (barrier != null) barrier.SetActive(true);
        }

        foreach (var canvas in countdownCanvases)
        {
            if (canvas != null) canvas.alpha = 1;
        }

        if (!usePhotonSync)
        {
            startTime = Time.time + prepareTime;
            countdownStarted = true;
        }
    }

    public override void OnJoinedRoom()
    {
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

        string textToShow;

        if (timeLeft > 3) textToShow = "Ready?";
        else if (timeLeft > 2)
        {
            textToShow = "3";
            if (!countSoundPlayed)
            {
                AudioManager.Instance.PlaySFX(SFXType.Count);
                countSoundPlayed = true;
            }
        }
        else if (timeLeft > 1) textToShow = "2";
        else if (timeLeft > 0) textToShow = "1";
        else
        {
            StartGame();
            return;
        }

        foreach (var txt in countdownTexts)
        {
            if (txt != null) txt.text = textToShow;
        }
    }

    void StartGame()
    {
        if (gameStarted) return;
        gameStarted = true;

        foreach (var txt in countdownTexts)
        {
            if (txt != null) txt.text = "GO!";
        }

        foreach (var barrier in startBarriers)
        {
            if (barrier != null) barrier.SetActive(false);
        }

        if (usePhotonSync)
        {
            photonView.RPC(nameof(RPC_OnGameStarted), RpcTarget.MasterClient);
        }
        else
        {
            StartBossLocal();
        }

        StartCoroutine(HideAllUI());
    }

    IEnumerator HideAllUI()
    {
        yield return new WaitForSeconds(0.8f);
        float t = 0f;
        while (t < 0.5f)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, t / 0.5f);
            foreach (var canvas in countdownCanvases)
            {
                if (canvas != null) canvas.alpha = alpha;
            }
            yield return null;
        }
        foreach (var canvas in countdownCanvases)
        {
            if (canvas != null) canvas.alpha = 0;
        }
    }

    [PunRPC]
    void RPC_OnGameStarted()
    {
        StartBossLocal();
    }

    void StartBossLocal()
    {
        if (!enableBoss) return;
        BossController boss = FindObjectOfType<BossController>();
        if (boss != null) boss.StartBoss();
    }

    public bool CheckGameStart()
    {
        return gameStarted;
    }
}