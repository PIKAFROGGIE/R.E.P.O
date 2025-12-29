using UnityEngine;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour
{
    public static TimerUI Instance;

    public Text timerText;

    void Awake()
    {
        Instance = this;
    }

    public void UpdateTime(float remaining)
    {
        int minutes = Mathf.FloorToInt(remaining / 60f);
        int seconds = Mathf.FloorToInt(remaining % 60f);

        timerText.text = $"{minutes:00}:{seconds:00}";
    }
}