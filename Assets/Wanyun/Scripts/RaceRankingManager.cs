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
    public float raceDuration = 120f; // 2 minutes

    float raceEndTime;
    bool raceEnded = false;

    // 到达终点的顺序
    Dictionary<Player, float> finishTimeDict = new Dictionary<Player, float>();

    void Awake()
    {
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

        if (Time.time >= raceEndTime)
        {
            raceEnded = true;
            CalculateFinalRanking();
        }
    }

    public void PlayerReachedFinish(Player player)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (finishTimeDict.ContainsKey(player))
            return;

        finishTimeDict[player] = Time.time;
        Debug.Log($"{player.NickName} reached finish");

        // 如果所有人都到了，提前结算?????这里逻辑有问题，可能会导致未到终点的玩家无法排名
        if (finishTimeDict.Count == PhotonNetwork.PlayerList.Length) 
        {
            raceEnded = true;
            CalculateFinalRanking();
        }
    }

    void CalculateFinalRanking()
    {
        List<Player> allPlayers = PhotonNetwork.PlayerList.ToList();

        // 已到终点的
        var finishedPlayers = finishTimeDict
            .OrderBy(kv => kv.Value)
            .Select(kv => kv.Key)
            .ToList();

        // 未到终点的
        var unfinishedPlayers = allPlayers
            .Where(p => !finishTimeDict.ContainsKey(p))
            .OrderBy(p => GetDistanceToFinish(p))
            .ToList();

        // 合并最终排名
        List<Player> finalRanking = new List<Player>();
        finalRanking.AddRange(finishedPlayers);
        finalRanking.AddRange(unfinishedPlayers);

        Debug.Log("Final Ranking:");
        for (int i = 0; i < finalRanking.Count; i++)
        {
            Debug.Log($"{i + 1}. {finalRanking[i].NickName}");
        }

        // 🔥 把排名交给积分系统
        //RoundScoreManager.Instance.EndRound(finalRanking);
    }

    float GetDistanceToFinish(Player player)
    {
        if (player.TagObject == null)
            return float.MaxValue;

        GameObject playerObj = player.TagObject as GameObject;
        return Vector3.Distance(playerObj.transform.position, finishPoint.position);
    }

    [PunRPC]
    public void RPC_PlayerReachedFinish(Player player)
    {
        PlayerReachedFinish(player);
    }

}
