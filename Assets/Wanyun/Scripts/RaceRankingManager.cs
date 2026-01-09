using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class RaceRankingManager : MonoBehaviourPunCallbacks
{
    public static RaceRankingManager Instance;

    [Header("Race Settings")]
    public Transform finishPoint;
    public string rankingSceneName = "RankingScene";
    public float delayBeforeLoadRankingScene = 3f; 

    private bool raceEnded = false;

    // 记录已到终点玩家及到达时间（Master 侧）
    private Dictionary<Player, float> finishTimeDict = new Dictionary<Player, float>();

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // 由 GameEndManager 在“时间到”时调用（Master only）
    public void OnRaceTimeUp()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        EndRace();
    }

    // RPC：由 FinishLineTrigger 发送到 Master
    [PunRPC]
    public void RPC_PlayerReachedFinish(Player player)
    {
        PlayerReachedFinish(player);
    }

    // 记录玩家到达终点（Master only）
    public void PlayerReachedFinish(Player player)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (raceEnded)
            return;

        if (finishTimeDict.ContainsKey(player))
            return;

        finishTimeDict[player] = Time.time;
        Debug.Log(player.NickName + " reached finish");

        // 若所有玩家都到达，提前结束
        if (finishTimeDict.Count == PhotonNetwork.PlayerList.Length)
        {
            EndRace();
        }
    }

    // 统一结束比赛入口（只会执行一次）
    void EndRace()
    {
        if (raceEnded)
            return;

        raceEnded = true;

        List<Player> finalRanking = CalculateFinalRanking();

        RaceResultCache.FinalRanking = finalRanking;

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

    // 计算最终排名（到达优先，其次按距离）
    List<Player> CalculateFinalRanking()
    {
        List<Player> allPlayers = PhotonNetwork.PlayerList.ToList();

        // 已到终点：按到达时间
        var finishedPlayers = finishTimeDict
            .OrderBy(kv => kv.Value)
            .Select(kv => kv.Key)
            .ToList();

        // 未到终点：按距离终点
        var unfinishedPlayers = allPlayers
            .Where(p => !finishTimeDict.ContainsKey(p))
            .OrderBy(p => GetDistanceToFinish(p))
            .ToList();

        List<Player> finalRanking = new List<Player>();
        finalRanking.AddRange(finishedPlayers);
        finalRanking.AddRange(unfinishedPlayers);

        Debug.Log("Final Ranking:");
        for (int i = 0; i < finalRanking.Count; i++)
        {
            Debug.Log((i + 1) + ". " + finalRanking[i].NickName);
        }

        return finalRanking;
    }

    float GetDistanceToFinish(Player player)
    {
        if (player == null || player.TagObject == null)
            return float.MaxValue;

        GameObject playerObj = player.TagObject as GameObject;
        if (playerObj == null || finishPoint == null)
            return float.MaxValue;

        return Vector3.Distance(playerObj.transform.position, finishPoint.position);
    }
}
