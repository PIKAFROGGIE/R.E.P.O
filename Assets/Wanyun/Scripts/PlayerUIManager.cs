using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text winText;
    public Text timeText;

    public float winTextShowDuration = 3f;

    void Awake()
    {
        // È·±£³õÊ¼×´Ì¬
        if (winText != null)
            winText.gameObject.SetActive(false);
    }

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


    public void UpdateCountdown(int seconds)
    {
        if (timeText != null)
            timeText.text = seconds.ToString();
    }

    public void ShowGameOver()
    {
        if (timeText != null)
            timeText.text = "GAME OVER";
    }
}
