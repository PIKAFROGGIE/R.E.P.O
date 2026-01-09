using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SceneInfo
{
    public string sceneName;    // 场景文件名
    public string displayName;  // UI 显示名称
    public string rule;         // 场景规则
    public Sprite image;        // 对应图片
}

public class LoadingGameManager : MonoBehaviourPunCallbacks
{
    [Header("UI")]
    public TMP_Text sceneNameText;
    public TMP_Text sceneRuleText;
    public Image sceneImage;

    [Header("场景配置")]
    public List<SceneInfo> scenes = new List<SceneInfo>();

    private const string CURRENT_SCENE_KEY = "CurrentScene";
    private const string USED_SCENES_KEY = "UsedScenes";
    private const int TOTAL_ROUNDS = 3;

    private int currentRound = 0;
    private bool isRunning = false;

    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        if (PhotonNetwork.IsMasterClient)
        {
            // Master 直接启动流程
            StartCoroutine(MasterRoundsCoroutine());
        }
        else
        {
            // Client 持续显示 LoadingScene UI
            StartCoroutine(ClientShowLoadingUI());
        }
    }

    // ===============================
    // MasterClient 控制游戏流程
    // ===============================
    IEnumerator MasterRoundsCoroutine()
    {
        if (isRunning) yield break;
        isRunning = true;

        while (currentRound < TOTAL_ROUNDS)
        {
            // 1️⃣ 选择关卡
            string selectedScene = PickRandomScene();
            if (string.IsNullOrEmpty(selectedScene))
                yield break;

            // 2️⃣ 更新 CustomProperties，让客户端显示 UI
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
            {
                { CURRENT_SCENE_KEY, selectedScene }
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);

            // 3️⃣ 等待一帧确保 CustomProperties 同步
            yield return null;

            // 4️⃣ 等待 LoadingScene 加载完成（Master 可能在 LoadingScene）
            if (SceneManager.GetActiveScene().name != "LoadingScene")
            {
                PhotonNetwork.LoadLevel("LoadingScene");
                yield return new WaitUntil(() => SceneManager.GetActiveScene().name == "LoadingScene");
            }

            // 5️⃣ 显示关卡 UI 并等待 5 秒
            ShowSceneInfo(selectedScene);
            yield return new WaitForSeconds(5f);

            // 6️⃣ 清理玩家
            PhotonView pv = PhotonView.Get(this);
            if (pv != null)
                pv.RPC("RPC_CleanupPlayer", RpcTarget.All);

            // 7️⃣ 加载游戏场景
            PhotonNetwork.LoadLevel(selectedScene);

            // 等待游戏场景加载完成
            yield return new WaitUntil(() => SceneManager.GetActiveScene().name == selectedScene);

            currentRound++;
        }

        isRunning = false;
        Debug.Log("三轮游戏结束");
    }

    // ===============================
    // Client 持续显示 LoadingScene UI
    // ===============================
    IEnumerator ClientShowLoadingUI()
    {
        while (true)
        {
            if (PhotonNetwork.CurrentRoom != null &&
                PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(CURRENT_SCENE_KEY))
            {
                string sceneName = PhotonNetwork.CurrentRoom.CustomProperties[CURRENT_SCENE_KEY] as string;
                ShowSceneInfo(sceneName);
            }
            yield return null;
        }
    }

    // ===============================
    // 随机选择未使用关卡
    // ===============================
    string PickRandomScene()
    {
        if (!PhotonNetwork.IsMasterClient) return null;

        List<string> usedScenes = GetUsedScenes();
        List<SceneInfo> available = new List<SceneInfo>();

        foreach (var s in scenes)
            if (!usedScenes.Contains(s.sceneName))
                available.Add(s);

        if (available.Count == 0) return null;

        SceneInfo selected = available[Random.Range(0, available.Count)];
        usedScenes.Add(selected.sceneName);

        // 更新已使用关卡
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
        {
            { USED_SCENES_KEY, string.Join(",", usedScenes) }
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);

        return selected.sceneName;
    }

    // ===============================
    // UI 显示关卡信息
    // ===============================
    void ShowSceneInfo(string sceneName)
    {
        SceneInfo info = scenes.Find(s => s.sceneName == sceneName);
        if (info != null)
        {
            sceneNameText.text = info.displayName;
            sceneRuleText.text = info.rule;
            sceneImage.sprite = info.image;
        }
        else
        {
            sceneNameText.text = sceneName;
            sceneRuleText.text = "";
            sceneImage.sprite = null;
        }
    }

    // ===============================
    // 获取已使用关卡
    // ===============================
    List<string> GetUsedScenes()
    {
        if (!PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(USED_SCENES_KEY))
            return new List<string>();

        string data = PhotonNetwork.CurrentRoom.CustomProperties[USED_SCENES_KEY] as string;
        return new List<string>(data.Split(','));
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey(CURRENT_SCENE_KEY))
        {
            string sceneName = propertiesThatChanged[CURRENT_SCENE_KEY] as string;
            ShowSceneInfo(sceneName);
        }
    }

    // ===============================
    // RPC 清理玩家
    // ===============================
    [PunRPC]
    void RPC_CleanupPlayer()
    {
        if (PhotonNetwork.LocalPlayer.TagObject != null)
        {
            GameObject player = PhotonNetwork.LocalPlayer.TagObject as GameObject;
            if (player != null)
                Destroy(player);
            PhotonNetwork.LocalPlayer.TagObject = null;
        }
    }
}
