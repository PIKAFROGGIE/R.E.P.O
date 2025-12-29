using UnityEngine;
using Photon.Pun;

public class GameTimer : MonoBehaviourPunCallbacks
{
    public static GameTimer Instance;

    [Header("Timer")]
    public float gameDuration = 120f; // 2 分钟

    double startTime;          // Photon 时间
    bool gameStarted = false;
    bool gameEnded = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            startTime = PhotonNetwork.Time;
            photonView.RPC(nameof(RPC_StartGame), RpcTarget.All, startTime);
        }
    }

    [PunRPC]
    void RPC_StartGame(double serverStartTime)
    {
        startTime = serverStartTime;
        gameStarted = true;
        gameEnded = false;
    }

    void Update()
    {
        if (!gameStarted || gameEnded) return;

        float elapsed = (float)(PhotonNetwork.Time - startTime);
        float remaining = Mathf.Max(0f, gameDuration - elapsed);

        // 每帧更新 UI
        TimerUI.Instance?.UpdateTime(remaining);

        if (remaining <= 0f)
        {
            EndGame();
        }
    }

    void EndGame()
    {
        if (gameEnded) return;
        gameEnded = true;

        photonView.RPC(nameof(RPC_TimeUp), RpcTarget.All);
    }

    [PunRPC]
    void RPC_TimeUp()
    {
        // 通知所有玩家：时间到
        PlayerMovementCheck[] players = FindObjectsOfType<PlayerMovementCheck>();
        foreach (var p in players)
        {
            p.OnTimeUp();
        }
    }
}
