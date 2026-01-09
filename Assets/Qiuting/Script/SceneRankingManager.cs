using System.Linq;
using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class SceneRankingManager : MonoBehaviourPun
{
    [Header("Ranking Scene Settings")]
    public string rankingSceneName = "RankingScene";
    public float delayBeforeLoadRankingScene = 3f;

    private bool rankingCalculated = false;

    /// <summary>
    /// 在游戏结束时调用（例如倒计时结束 / 胜利条件触发）
    /// </summary>
    public void CalculateRanking()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.Log("SceneRankingManager: Not MasterClient, skipping ranking.");
            return;
        }

        if (rankingCalculated)
            return;

        rankingCalculated = true;

        Debug.Log("🏆 游戏结束排名（MasterClient 计算）：");

        // 1️⃣ 按 score 从高到低排序
        var sortedPlayers = PhotonNetwork.PlayerList
            .OrderByDescending(p =>
            {
                object scoreObj;
                if (p.CustomProperties.TryGetValue("score", out scoreObj))
                    return (int)scoreObj;
                return 0;
            })
            .ToArray();

        // 2️⃣ Debug 输出（方便你检查）
        for (int i = 0; i < sortedPlayers.Length; i++)
        {
            int score = 0;
            object scoreObj;
            if (sortedPlayers[i].CustomProperties.TryGetValue("score", out scoreObj))
                score = (int)scoreObj;

            Debug.Log($"Rank {i + 1}: {sortedPlayers[i].NickName} → {score}");
        }

        // 3️⃣ 缓存排名结果（给 RankingScene 用）
        RaceResultCache.FinalRanking = sortedPlayers.ToList();


        // 4️⃣ 清理 TagObject，避免跨场景残留
        foreach (var p in PhotonNetwork.PlayerList)
        {
            p.TagObject = null;
        }

        // 5️⃣ 延迟切换到排名场景
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
}
