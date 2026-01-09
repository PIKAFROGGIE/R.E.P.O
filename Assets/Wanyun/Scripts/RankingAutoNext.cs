using UnityEngine;
using System.Collections;

public class RankingAutoNext : MonoBehaviour
{
    [Header("Delay")]
    public float waitTime = 5f;

    public static RankingAutoNext Instance;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void PrepareForNextRound()
    {
        StartCoroutine(AutoNext());
    }

    IEnumerator AutoNext()
    {
        yield return new WaitForSeconds(waitTime);

        if (GameOverManager.Instance != null)
        {
            GameOverManager.Instance.EndGame();
        }
        else
        {
            Debug.LogError("GameOverManager.Instance is null");
        }
    }
}
