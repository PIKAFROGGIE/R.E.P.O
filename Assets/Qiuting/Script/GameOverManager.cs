using Photon.Pun;
using ExitGames.Client.Photon;
using UnityEngine;

public class GameOverManager : MonoBehaviourPun
{
    private const string CURRENT_SCENE_KEY = "CurrentScene";

    public static GameOverManager Instance;

    void Awake()
    {
        Instance = this;
    }

    // ===============================
    // 所有关卡只调用这个
    // ===============================
    public void EndGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            MasterEndGame();
        }
        else
        {
            photonView.RPC(nameof(RPC_RequestEndGame), RpcTarget.MasterClient);
        }
    }

    // Client -> Master
    [PunRPC]
    void RPC_RequestEndGame()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        MasterEndGame();
    }

    // 真正结束游戏（只在 Master）
    void MasterEndGame()
    {
        Debug.Log("Game Over → LoadingScene");

        Hashtable props = new Hashtable
        {
            { CURRENT_SCENE_KEY, "" }
        };

        PhotonNetwork.CurrentRoom.SetCustomProperties(props);

        PhotonNetwork.LoadLevel("LoadingScene");
    }
}
