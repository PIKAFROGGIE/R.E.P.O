using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine;

public class ScoreChangeListener : MonoBehaviourPunCallbacks
{
    public override void OnPlayerPropertiesUpdate(
        Player targetPlayer,
        Hashtable changedProps
    )
    {
        if (changedProps.ContainsKey("Score"))
        {
            if (LeaderboardUIManager.Instance != null)
            {
                LeaderboardUIManager.Instance.RefreshLeaderboard();
            }
        }
    }
}
