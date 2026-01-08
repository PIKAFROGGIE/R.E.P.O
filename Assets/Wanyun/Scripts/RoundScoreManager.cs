using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RoundScoreManager : MonoBehaviourPunCallbacks
{
    public static RoundScoreManager Instance;

    public int currentRound = 1;
    public int maxRound = 3;

    private readonly int[] rankRewards = { 10000, 5000, 2000 };

    void Awake()
    {
        Instance = this;
    }

    public void EndRound(List<Player> rankedPlayers)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        Debug.Log($"Round {currentRound} End");

        for (int i = 0; i < rankedPlayers.Count; i++)
        {
            int reward = (i < rankRewards.Length) ? rankRewards[i] : 0;

            AddScore(rankedPlayers[i], reward);
        }

        photonView.RPC(nameof(RPC_SyncScores), RpcTarget.All);

        currentRound++;

        if (currentRound > maxRound)
        {
            Debug.Log("Game Finished - Final Ranking");
        }
    }

    void AddScore(Player player, int add)
    {
        int oldScore = player.CustomProperties.ContainsKey("Score")
            ? (int)player.CustomProperties["Score"]
            : 0;

        Hashtable ht = new Hashtable();
        ht["Score"] = oldScore + add;

        player.SetCustomProperties(ht);
    }


    [PunRPC]
    void RPC_SyncScores()
    {
        // ¿Í»§¶ËË¢ÐÂ UI
        Debug.Log("Scores synced");
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
