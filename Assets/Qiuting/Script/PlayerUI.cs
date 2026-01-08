using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        currentTime = totalTime;
        if (timerText != null) timerText.color = Color.white;

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
            {
                timerText.color = Color.red;
            }
            UpdateTimerUI();
        }
        else
        {
            hasEnded = true;
            GameOverManager.Instance.EndGame();

        }
    }
  
    void UpdateTimerUI()
    {
        if (timerText == null) return;
        float displayTime = Mathf.Max(currentTime, 0f);
        int minutes = Mathf.FloorToInt(displayTime / 60f);
        int seconds = Mathf.FloorToInt(displayTime % 60f);
        timerText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
    }

    public float GetCurrentTime()
    {
        return currentTime;
    }


}
