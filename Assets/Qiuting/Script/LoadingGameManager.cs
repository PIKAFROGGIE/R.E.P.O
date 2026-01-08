using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon; // Photon Hashtable
using TMPro;
using UnityEngine.UI;

// ===============================
// 可序列化类，用于 Inspector 管理每个关卡
// ===============================
[System.Serializable]
public class SceneInfo
{
    public string sceneName;      // 场景文件名，例如 "GameScene_A"
    public string displayName;    // UI上显示的名字，例如 "Map1 - Pulse Corridor"
    public string rule;           // 场景规则，例如 "规则 A：存活到最后"
    public Sprite image;          // 对应图片（直接拖入Inspector）
}

public class LoadingGameManager : MonoBehaviourPunCallbacks
{
    [Header("UI")]
    public TMP_Text sceneNameText;
    public TMP_Text sceneRuleText;
    public Image sceneImage;

    [Header("场景配置")]
    public List<SceneInfo> scenes = new List<SceneInfo>();

    private const string USED_SCENES_KEY = "UsedScenes";
    private const string CURRENT_SCENE_KEY = "CurrentScene";
    private const int TOTAL_ROUNDS = 3; // 玩三轮

    void Start()
    {
        // 开启自动场景同步（Photon 会自动让客户端加载 Master Client 的场景）
        PhotonNetwork.AutomaticallySyncScene = true;

        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(StartGameSequence());
        }
        else
        {
            // 客户端等待 Master Client 设置 CURRENT_SCENE_KEY
            StartCoroutine(WaitForSceneSelection());
        }
    }

    // ===============================
    // Master Client 主流程协程
    // ===============================
    IEnumerator StartGameSequence()
    {
        for (int round = 0; round < TOTAL_ROUNDS; round++)
        {
            string selectedScene = PickRandomScene();
            if (string.IsNullOrEmpty(selectedScene))
                yield break;

            ShowSceneInfo(selectedScene);

            yield return new WaitForSeconds(3f);

            // ✅ Master 明确标记“当前正在玩的关卡”
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
{
    { CURRENT_SCENE_KEY, selectedScene }
};

            PhotonNetwork.CurrentRoom.SetCustomProperties(props);

            PhotonNetwork.LoadLevel(selectedScene);

            // ✅ 等待 GameOverManager 把 CURRENT_SCENE_KEY 清空
            yield return new WaitUntil(() =>
            {
                if (!PhotonNetwork.InRoom) return true;
                if (!PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(CURRENT_SCENE_KEY)) return false;

                string current = PhotonNetwork.CurrentRoom.CustomProperties[CURRENT_SCENE_KEY] as string;
                return string.IsNullOrEmpty(current);
            });
        }

        Debug.Log("三轮游戏结束");
    }

    // ===============================
    // 客户端等待 Master Client 设置关卡
    // ===============================
    IEnumerator WaitForSceneSelection()
    {
        while (true)
        {
            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(CURRENT_SCENE_KEY))
            {
                string sceneName = PhotonNetwork.CurrentRoom.CustomProperties[CURRENT_SCENE_KEY] as string;
                ShowSceneInfo(sceneName);
                break;
            }
            yield return null;
        }
    }

    // ===============================
    // 随机选择关卡（仅 Master Client）
    // ===============================
    string PickRandomScene()
    {
        if (!PhotonNetwork.IsMasterClient) return null; // 仅 Master Client 选场景

        List<string> usedScenes = GetUsedScenes();

        List<SceneInfo> available = new List<SceneInfo>();
        foreach (SceneInfo s in scenes)
        {
            if (!usedScenes.Contains(s.sceneName))
                available.Add(s);
        }

        if (available.Count == 0)
            return null;

        SceneInfo selected = available[Random.Range(0, available.Count)];
        usedScenes.Add(selected.sceneName);

        // 更新 Photon Room CustomProperties
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
        props[USED_SCENES_KEY] = string.Join(",", usedScenes);
        props[CURRENT_SCENE_KEY] = selected.sceneName;
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);

        return selected.sceneName;
    }

    // ===============================
    // 显示 UI 信息
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
    // 获取已用关卡
    // ===============================
    List<string> GetUsedScenes()
    {
        if (!PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(USED_SCENES_KEY))
            return new List<string>();

        string data = PhotonNetwork.CurrentRoom.CustomProperties[USED_SCENES_KEY] as string;
        return new List<string>(data.Split(','));
    }

    // ===============================
    // 监听 Room CustomProperties 更新（保证客户端 UI 同步）
    // ===============================
    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey(CURRENT_SCENE_KEY))
        {
            string sceneName = propertiesThatChanged[CURRENT_SCENE_KEY] as string;
            ShowSceneInfo(sceneName);
        }
    }
}
