using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RankItemUI : MonoBehaviour
{
    public TMP_Text rankText;
    public TMP_Text nameText;
    public TMP_Text scoreText;

    public Image highlightBG;

    public Color firstColor;
    public Color secondColor;
    public Color thirdColor;
    public Color normalColor;

    public void Setup(int rank, string playerName, int score)
    {
        rankText.text = rank.ToString();
        nameText.text = playerName;
        scoreText.text = score.ToString("N0");

        // 默认
        highlightBG.color = normalColor;

        // 前三名样式
        if (rank == 1)
        {
            highlightBG.color = firstColor;
        }
        else if (rank == 2)
        {
            highlightBG.color = secondColor;
        }
        else if (rank == 3)
        {
            highlightBG.color = thirdColor;
        }
    }
}
