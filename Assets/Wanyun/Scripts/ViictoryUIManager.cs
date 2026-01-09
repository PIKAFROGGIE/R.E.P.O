using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Linq;

public class VictoryUIManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text rankText;
    public TMP_Text scoreText;

    void Start()
    {
        Player localPlayer = PhotonNetwork.LocalPlayer;

        // ×Ü·Ö
        int score = localPlayer.CustomProperties.ContainsKey("Score")
            ? (int)localPlayer.CustomProperties["Score"]
            : 0;

        scoreText.text = score.ToString();

        // ÅÅÃû
        if (RaceResultCache.FinalRanking != null)
        {
            int rank = RaceResultCache.FinalRanking
                .FindIndex(p => p == localPlayer) + 1;

            rankText.text = rank > 0 ? rank.ToString() : "-";
        }
        else
        {
            rankText.text = "-";
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
