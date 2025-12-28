using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class TestKey : MonoBehaviourPunCallbacks
{
    [Header("Hotkey Settings")]
    public KeyCode hotkey = KeyCode.Alpha1;
    public string targetSceneName = "Yunyi";

    void Start()
    {
        // 关键：开启场景同步
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Update()
    {
        // 只有房主可以触发
        if (!PhotonNetwork.IsMasterClient) return;

        if (Input.GetKeyDown(hotkey))
        {
            PhotonNetwork.LoadLevel(targetSceneName);
        }
    }
}
