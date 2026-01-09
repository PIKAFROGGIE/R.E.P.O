using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class TestKey : MonoBehaviourPunCallbacks
{
    [Header("Hotkey Settings")]
    public KeyCode hotkey1 = KeyCode.Alpha1;
    public KeyCode hotkey2 = KeyCode.Alpha2;
    public KeyCode hotkey3 = KeyCode.Alpha3;
    public KeyCode hotkey4 = KeyCode.Alpha4;
    public KeyCode hotkey5 = KeyCode.Alpha5;
    public string targetSceneName1 = "Yunyi";
    public string targetSceneName2 = "Wanyun";
    public string targetSceneName3 = "Qiuting";
    public string targetSceneName4 = "Bryan";
    public string targetSceneName5 = "Josh";

    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (Input.GetKeyDown(hotkey1))
        {
            PhotonNetwork.LoadLevel(targetSceneName1);
        }

        if (Input.GetKeyDown(hotkey2))
        {
            PhotonNetwork.LoadLevel(targetSceneName2);
        }

        if (Input.GetKeyDown(hotkey3))
        {
            PhotonNetwork.LoadLevel(targetSceneName3);
        }

        if (Input.GetKeyDown(hotkey4))
        {
            PhotonNetwork.LoadLevel(targetSceneName4);
        }

        if (Input.GetKeyDown(hotkey5))
        {
            PhotonNetwork.LoadLevel(targetSceneName5);
        }
    }
}
