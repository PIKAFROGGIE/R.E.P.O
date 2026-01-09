using UnityEngine;
using Photon.Pun;

public class GameEndManager : MonoBehaviourPunCallbacks
{
    public static GameEndManager Instance;

    [Header("Mode")]
    public bool usePhotonSync = true;

    [Header("Global Timer")]
    public float totalGameTime = 120f;

    private double endTime;
    private bool timerStarted;
    private bool gameEnded;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        if (!usePhotonSync)
        {
            endTime = Time.time + totalGameTime;
            timerStarted = true;
            return;
        }

        if (PhotonNetwork.IsMasterClient)
        {
            endTime = PhotonNetwork.Time + totalGameTime;
            photonView.RPC(nameof(RPC_StartGlobalTimer), RpcTarget.All, endTime);
        }
    }

    void Update()
    {
        if (!timerStarted || gameEnded)
            return;

        double now = usePhotonSync ? PhotonNetwork.Time : Time.time;
        double timeLeft = endTime - now;

        if (timeLeft > 0)
        {
            if (PlayerUIManager.Instance != null)
            {
                PlayerUIManager.Instance.UpdateCountdown(
                    Mathf.CeilToInt((float)timeLeft)
                );
            }
        }
        else
        {
            gameEnded = true;

            if (PlayerUIManager.Instance != null)
            {
                PlayerUIManager.Instance.ShowGameOver();
            }

            if (PhotonNetwork.IsMasterClient && RaceRankingManager.Instance != null)
            {
                RaceRankingManager.Instance.OnRaceTimeUp();
            }
        }
    }

    [PunRPC]
    void RPC_StartGlobalTimer(double syncedEndTime)
    {
        endTime = syncedEndTime;
        timerStarted = true;
    }
}
