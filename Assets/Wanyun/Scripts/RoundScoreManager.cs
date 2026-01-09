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

    // 第 1 / 2 / 3 名奖励
    private readonly int[] rankRewards = { 10000, 5000, 2000 };

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
        // RankingScene 进入时自动结算本轮
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (RaceResultCache.FinalRanking == null)
        {
            Debug.LogError("RaceResultCache.FinalRanking is null");
            return;
        }

        EndRound(RaceResultCache.FinalRanking);
    }

    // 只由 MasterClient 调用
    void EndRound(List<Player> rankedPlayers)
    {
        Debug.Log($"Round {currentRound} End");

        for (int i = 0; i < rankedPlayers.Count; i++)
        {
            int reward = (i < rankRewards.Length) ? rankRewards[i] : 0;
            AddScore(rankedPlayers[i], reward);
        }

        // 通知所有客户端分数已更新
        photonView.RPC(nameof(RPC_SyncScores), RpcTarget.All);

        currentRound++;

        // 保存轮次，给下一场用
        RaceResultCache.CurrentRound = currentRound;

        if (currentRound > maxRound)
        {
            Debug.Log("All rounds finished");
            // 最终排名 Scene 或结算 Scene
            // PhotonNetwork.LoadLevel("FinalResultScene");
        }
        else
        {
            
            RankingAutoNext.Instance.PrepareForNextRound();
            Debug.Log("Waiting for next round");
        }
    }

    void AddScore(Player player, int add)
    {
        int oldScore = player.CustomProperties.ContainsKey("Score")
            ? (int)player.CustomProperties["Score"]
            : 0;

        Hashtable ht = new Hashtable
        {
            ["Score"] = oldScore + add
        };

        player.SetCustomProperties(ht);
    }

    [PunRPC]
    void RPC_SyncScores()
    {
        Debug.Log("Scores synced");

        if (LeaderboardUIManager.Instance == null)
        {
            Debug.LogError("LeaderboardUIManager.Instance is null");
            return;
        }

        LeaderboardUIManager.Instance.RefreshLeaderboard();
    }


    // 给排行榜 UI 用
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
