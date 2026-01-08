using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    public static PlayerUIManager Instance;

    [Header("UI Elements")]
    public TMP_Text winText;
    public Text timeText;

    public float winTextShowDuration = 3f;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (winText != null)
            winText.gameObject.SetActive(false);

        if (timeText != null)
            timeText.gameObject.SetActive(true);
    }

    // ===== 全局倒计时显示 =====
    public void UpdateCountdown(int seconds)
    {
        if (timeText == null) return;

        int minutes = seconds / 60;
        int sec = seconds % 60;
        timeText.text = $"{minutes:0}:{sec:00}";

        if (seconds <= 10)
            timeText.color = Color.red;
    }

    // ===== 玩家过线 =====
    public void ShowWinText()
    {
        if (winText == null) return;

        winText.text = "YOU WIN!";
        winText.gameObject.SetActive(true);

        StopCoroutine(nameof(HideWinTextAfterDelay));
        StartCoroutine(HideWinTextAfterDelay());
    }

    IEnumerator HideWinTextAfterDelay()
    {
        yield return new WaitForSeconds(winTextShowDuration);

        if (winText != null)
            winText.gameObject.SetActive(false);
    }

    // ===== 游戏结束 =====
    public void ShowGameOver()
    {
        if (timeText != null)
            timeText.text = "GAME OVER";
    }
}
