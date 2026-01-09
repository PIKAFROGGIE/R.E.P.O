using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class RaceRankingManager : MonoBehaviourPunCallbacks
{
    public static RaceRankingManager Instance;

    [Header("Race Settings")]
    public Transform finishPoint;
    public float raceDuration = 120f;

    private float raceEndTime;
    private bool raceEnded = false;

    // 记录已到终点玩家及到达时间
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

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            raceEndTime = Time.time + raceDuration;
        }
    }

    void Update()
    {
        if (!PhotonNetwork.IsMasterClient || raceEnded)
            return;

        // 时间到，强制结算（包含未到终点玩家）
        if (Time.time >= raceEndTime)
        {
            EndRace();
        }
    }

    // RPC：由 FinishLineTrigger 调用，仅发送到 MasterClient
    [PunRPC]
    public void RPC_PlayerReachedFinish(Player player)
    {
        PlayerReachedFinish(player);
    }

    // 记录玩家到达终点
    public void PlayerReachedFinish(Player player)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (finishTimeDict.ContainsKey(player))
            return;

        finishTimeDict[player] = Time.time;
        Debug.Log(player.NickName + " reached finish");

        // 所有人都到达，提前结束比赛
        if (finishTimeDict.Count == PhotonNetwork.PlayerList.Length)
        {
            EndRace();
        }
    }

    // 统一结束比赛入口
    void EndRace()
    {
        if (raceEnded)
            return;

        raceEnded = true;
        CalculateFinalRanking();
    }

    // 计算最终排名
    void CalculateFinalRanking()
    {
        List<Player> allPlayers = PhotonNetwork.PlayerList.ToList();

        // 已到终点：按到达时间排序
        var finishedPlayers = finishTimeDict
            .OrderBy(kv => kv.Value)
            .Select(kv => kv.Key)
            .ToList();

        // 未到终点：按距离终点排序
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

        // 保存结果，给 RankingScene 用
        RaceResultCache.FinalRanking = finalRanking;
    }

    float GetDistanceToFinish(Player player)
    {
        if (player.TagObject == null)
            return float.MaxValue;

        GameObject playerObj = player.TagObject as GameObject;
        if (playerObj == null)
            return float.MaxValue;

        return Vector3.Distance(playerObj.transform.position, finishPoint.position);
    }
}
