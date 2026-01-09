using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RoundScoreManager : MonoBehaviourPunCallbacks
{
    public static RoundScoreManager Instance;

    [Header("Round Settings")]
    public int currentRound = 1;
    public int maxRound = 3;

    public const string ROOM_ROUND_KEY = "Round";

    private readonly int[] rankRewards = { 10000, 5000, 2000 };

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (PhotonNetwork.CurrentRoom != null &&
            PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(ROOM_ROUND_KEY))
        {
            currentRound = (int)PhotonNetwork.CurrentRoom.CustomProperties[ROOM_ROUND_KEY];
        }

        if (!PhotonNetwork.IsMasterClient)
            return;

        if (RaceResultCache.FinalRanking == null)
        {
            Debug.LogError("RaceResultCache.FinalRanking is null");
            return;
        }

        EndRound(RaceResultCache.FinalRanking);
    }

    void EndRound(List<Player> rankedPlayers)
    {
        Debug.Log($"Round {currentRound} End");

        for (int i = 0; i < rankedPlayers.Count; i++)
        {
            int reward = (i < rankRewards.Length) ? rankRewards[i] : 0;
            AddScore(rankedPlayers[i], reward);
        }

        currentRound++;

        Hashtable roomProps = new Hashtable
        {
            { ROOM_ROUND_KEY, currentRound }
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(roomProps);

        if (currentRound > maxRound)
        {
            Debug.Log("All rounds finished ¡ú VictoryScene");

            RankingAutoNext.Instance.LoadFinalRound();
        }
        else
        {
            RankingAutoNext.Instance.PrepareForNextRound();
        }
    }

    void AddScore(Player player, int add)
    {
        int oldScore = player.CustomProperties.ContainsKey("Score")
            ? (int)player.CustomProperties["Score"]
            : 0;

        Hashtable ht = new Hashtable
        {
            { "Score", oldScore + add }
        };

        player.SetCustomProperties(ht);
    }

    public List<Player> GetSortedPlayersByScore()
    {
        return PhotonNetwork.PlayerList
            .OrderByDescending(p =>
                p.CustomProperties.ContainsKey("Score")
                    ? (int)p.CustomProperties["Score"]
                    : 0)
            .ToList();
    }
}
