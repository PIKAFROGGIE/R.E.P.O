using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class LastManRanking : MonoBehaviourPunCallbacks
{
    public static LastManRanking Instance;

    [Header("Game Settings")]
    public string rankingSceneName = "RankingScene";
    public float delayBeforeLoadRankingScene = 3f;

    private bool gameEnded = false;

    private List<Player> deadPlayersLog = new List<Player>();

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // --- Called when Time runs out (from GameManager) ---
    public void OnRaceTimeUp()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        EndGame();
    }

    // --- RPC: Called by DieController when a player dies ---
    [PunRPC]
    public void RPC_ReportDeath(Player player)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        RegisterDeath(player);
    }

    // Master Client logic to record death
    void RegisterDeath(Player player)
    {
        if (gameEnded) return;
        if (deadPlayersLog.Contains(player)) return; // Already registered

        Debug.Log(player.NickName + " has died.");
        deadPlayersLog.Add(player);

        // Check if Game Should End (Last Man Standing)
        // If (Total Players - Dead Players) <= 1, we have a winner.
        int livingCount = PhotonNetwork.PlayerList.Length - deadPlayersLog.Count;

        if (livingCount <= 1)
        {
            EndGame();
        }
    }

    // Unified End Game Logic
    void EndGame()
    {
        if (gameEnded) return;
        gameEnded = true;

        List<Player> finalRanking = CalculateSurvivalRanking();

        // Save to cache for the next scene
        RaceResultCache.FinalRanking = finalRanking;

        // Cleanup references
        foreach (var p in PhotonNetwork.PlayerList)
        {
            p.TagObject = null;
        }

        StartCoroutine(LoadRankingSceneAfterDelay());
    }

    IEnumerator LoadRankingSceneAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeLoadRankingScene);

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(rankingSceneName);
        }
    }

    // --- CORE LOGIC: Calculate Rank based on Survival ---
    List<Player> CalculateSurvivalRanking()
    {
        List<Player> allPlayers = PhotonNetwork.PlayerList.ToList();
        List<Player> finalRanking = new List<Player>();

        // 1. Identify Survivors (People not in the dead log)
        var survivors = allPlayers
            .Where(p => !deadPlayersLog.Contains(p))
            .ToList();

        // 2. Add Survivors to the TOP of the list (Rank 1, Rank 2...)
        // If there are multiple survivors (Time Up), they share top spots.
        finalRanking.AddRange(survivors);

        // 3. Add Dead Players in REVERSE order
        // The last person added to deadPlayersLog lived the longest (just below survivors).
        // The first person added to deadPlayersLog died immediately (Last place).
        var sortedDeadPlayers = deadPlayersLog.AsEnumerable().Reverse().ToList();

        finalRanking.AddRange(sortedDeadPlayers);

        // Debug Output
        Debug.Log("--- Final Survival Ranking ---");
        for (int i = 0; i < finalRanking.Count; i++)
        {
            Debug.Log($"Rank {i + 1}: {finalRanking[i].NickName}");
        }

        return finalRanking;
    }
}