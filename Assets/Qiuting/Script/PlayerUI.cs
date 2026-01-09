using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerUI : MonoBehaviour
{
    public static PlayerUI Instance { get; private set; }

    [Header("Timer")]
    public Text timerText;
    public float totalTime = 300f;
    private float currentTime;

    public bool debugJumpTo10 = true;
    private bool hasEnded = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        currentTime = totalTime;
        if (timerText != null)
            timerText.color = Color.white;
    }

    void Update()
    {
        if (debugJumpTo10 && Input.GetKeyDown(KeyCode.J))
        {
            currentTime = 5f;
            Debug.Log("⚡ Debug：直接跳到倒计时 5 秒！");
        }

        if (hasEnded) return;

        if (currentTime > 0f)
        {
            currentTime -= Time.deltaTime;

            if (currentTime <= 10f && timerText != null)
                timerText.color = Color.red;

            UpdateTimerUI();
        }
        else
        {
            hasEnded = true;

            if (PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(DelayedRankingAndEnd());
            }
        }
    }

    void UpdateTimerUI()
    {
        if (timerText == null) return;

        float displayTime = Mathf.Max(currentTime, 0f);
        int minutes = Mathf.FloorToInt(displayTime / 60f);
        int seconds = Mathf.FloorToInt(displayTime % 60f);

        timerText.text = $"{minutes:0}:{seconds:00}";
    }



    public float GetCurrentTime()
    {
        return currentTime;
    }

    private IEnumerator DelayedRankingAndEnd()
    {
        // 延迟一帧或 0.2 秒确保所有玩家分数同步完成
        yield return new WaitForSeconds(0.2f);

        var rankingManager = FindObjectOfType<SceneRankingManager>();
        if (rankingManager != null)
        {
            rankingManager.CalculateRanking();
        }
        else
        {
            Debug.LogError("❌ SceneRankingManager NOT FOUND on MasterClient!");
        }

        //GameOverManager.Instance.EndGame();
    }   
}
