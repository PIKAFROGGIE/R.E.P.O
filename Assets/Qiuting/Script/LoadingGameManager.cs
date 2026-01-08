using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon; // Photon Hashtable
using TMPro;
using UnityEngine.UI;

public class LoadingGameManager : MonoBehaviourPunCallbacks
{
    [Header("UI")]
    public TMP_Text sceneNameText;
    public TMP_Text sceneRuleText;
    public Image sceneImage;

    [Header("Scene Data")]
    public List<string> allScenes = new List<string>()
    {
        "GameScene_A",
        "GameScene_B",
        "GameScene_C",
        "GameScene_D"
    };

    // 自定义显示名
    private Dictionary<string, string> sceneDisplayNames = new Dictionary<string, string>()
    {
        {"GameScene_A", "Map1 - Pulse Corridor"},
        {"GameScene_B", "Map2 - Hold the Crown "},
        {"GameScene_C", "Map3 - UFO Battle"},
        {"GameScene_D", "Map4 - Red Light, Green Light"}
    };

    // 自定义规则
    private Dictionary<string, string> sceneRules = new Dictionary<string, string>()
    {
        {"GameScene_A", "规则 A：存活到最后"},
        {"GameScene_B", "规则 B：积分最高获胜"},
        {"GameScene_C", "规则 C：团队对抗"},
        {"GameScene_D", "规则 D：限时挑战"}
    };

    private const string USED_SCENES_KEY = "UsedScenes";
    private const string CURRENT_SCENE_KEY = "CurrentScene";

    private const int TOTAL_ROUNDS = 3; // 玩三轮

    void Start()
    {
        StartCoroutine(StartGameSequence());
    }

    // ===============================
    // 主流程协程
    // ===============================
    IEnumerator StartGameSequence()
    {
        for (int round = 0; round < TOTAL_ROUNDS; round++)
        {
            string selectedScene = PickRandomScene();
            if (string.IsNullOrEmpty(selectedScene))
            {
                Debug.LogWarning("没有剩余关卡可选");
                yield break;
            }

            ShowSceneInfo(selectedScene);

            yield return new WaitForSeconds(5f); // 等待5秒

            PhotonNetwork.LoadLevel(selectedScene);

            // 等待关卡结束并返回 LoadingScene
            yield return new WaitUntil(() =>
                PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(CURRENT_SCENE_KEY) &&
                (PhotonNetwork.CurrentRoom.CustomProperties[CURRENT_SCENE_KEY] as string) != selectedScene
            );
        }

        // 三轮结束
        Debug.Log("三轮游戏结束，退出游戏");
        // Application.Quit(); // 或返回主菜单
    }

    // ===============================
    // 随机选择关卡并更新已用关卡
    // ===============================
    string PickRandomScene()
    {
        List<string> usedScenes = GetUsedScenes();

        List<string> available = new List<string>();
        foreach (string s in allScenes)
        {
            if (!usedScenes.Contains(s))
                available.Add(s);
        }

        if (available.Count == 0)
            return null;

        string selectedScene = available[Random.Range(0, available.Count)];
        usedScenes.Add(selectedScene);

        // ← 使用 Photon Hashtable 避免报错
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
        props[USED_SCENES_KEY] = string.Join(",", usedScenes);
        props[CURRENT_SCENE_KEY] = selectedScene;

        PhotonNetwork.CurrentRoom.SetCustomProperties(props);

        return selectedScene;
    }

    // ===============================
    // 显示UI
    // ===============================
    void ShowSceneInfo(string sceneName)
    {
        sceneNameText.text = sceneDisplayNames.ContainsKey(sceneName) ? sceneDisplayNames[sceneName] : sceneName;
        sceneRuleText.text = sceneRules.ContainsKey(sceneName) ? sceneRules[sceneName] : "";
        sceneImage.sprite = Resources.Load<Sprite>("SceneImages/" + sceneName);
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
}
