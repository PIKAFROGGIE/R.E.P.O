using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;

public class LeaderboardUIManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text roundTitleText;
    public Transform contentParent;
    public GameObject rankItemPrefab;

    [Header("Config")]
    public int maxShowCount = 10;

    List<GameObject> spawnedItems = new List<GameObject>();

    void OnEnable()
    {
        RefreshLeaderboard();
    }

    public void RefreshLeaderboard()
    {
        ClearItems();

        // Round БъЬт
        roundTitleText.text = $"Round {RoundScoreManager.Instance.currentRound - 1}";

        List<Player> players =
            RoundScoreManager.Instance.GetSortedPlayersByScore();

        for (int i = 0; i < players.Count && i < maxShowCount; i++)
        {
            Player p = players[i];

            GameObject item = Instantiate(rankItemPrefab, contentParent);
            spawnedItems.Add(item);

            RankItemUI ui = item.GetComponent<RankItemUI>();

            int score = p.CustomProperties.ContainsKey("Score")
                ? (int)p.CustomProperties["Score"]
                : 0;

            ui.Setup(
                i + 1,
                string.IsNullOrEmpty(p.NickName) ? "Player" : p.NickName,
                score
            );
        }
    }

    void ClearItems()
    {
        foreach (var go in spawnedItems)
            Destroy(go);

        spawnedItems.Clear();
    }
}
