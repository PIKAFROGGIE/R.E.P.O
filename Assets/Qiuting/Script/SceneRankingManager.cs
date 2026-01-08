using System.Linq;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class SceneRankingManager : MonoBehaviourPun
{
    public void CalculateRanking()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.Log("SceneRankingManager: Not MasterClient, skipping ranking.");
            return;
        }

        Debug.Log("🏆 游戏结束排名（MasterClient 计算）：");

        var sortedPlayers = PhotonNetwork.PlayerList
            .OrderByDescending(p =>
            {
                object scoreObj;
                if (p.CustomProperties.TryGetValue("score", out scoreObj))
                    return (int)scoreObj;
                return 0;
            })
            .ToArray();

        for (int i = 0; i < sortedPlayers.Length; i++)
        {
            int score = 0;
            object scoreObj;
            if (sortedPlayers[i].CustomProperties.TryGetValue("score", out scoreObj))
                score = (int)scoreObj;

            Debug.Log($"Rank {i + 1}: {sortedPlayers[i].NickName} → {score}");
        }
    }
}
