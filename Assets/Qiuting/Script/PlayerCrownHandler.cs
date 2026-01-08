using UnityEngine;

public class PlayerCrownHandler : MonoBehaviour
{
    private Crown crown;

    // ===== 新增：计分相关（不新增脚本）=====
    public int score = 0;
    private float scoreTimer = 0f;
    private bool isKing = false;

    private void Start()
    {
        crown = FindObjectOfType<Crown>();
        if (crown == null)
        {
            Debug.LogError("Crown not found in scene!");
        }
    }

    private void Update()
    {
        if (crown == null) return;

        // 如果我是国王
        if (crown.currentOwner == transform)
        {
            if (!isKing)
            {
                isKing = true;
                scoreTimer = 0f;
                Debug.Log($"{name} is now KING 👑");
            }

            scoreTimer += Time.deltaTime;

            if (scoreTimer >= 1f)
            {
                score += 1;
                scoreTimer = 0f;

                Debug.Log($"{name} +1 score → Current score: {score}");
            }
        }
        else
        {
            if (isKing)
            {
                isKing = false;
                scoreTimer = 0f;
                Debug.Log($"{name} is no longer king.");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 从地上捡皇冠
        if (other.CompareTag("Crown") && crown.currentOwner == null)
        {
            crown.AttachToPlayer(transform);
            Debug.Log($"{name} picked up the crown from the ground.");
            return;
        }

        if (!other.CompareTag("Player")) return;
        if (other.transform == transform) return; // 避免自己

        if (crown == null)
        {
            Debug.LogWarning("Crown is null!");
            return;
        }

        string currentOwnerName = crown.currentOwner ? crown.currentOwner.name : "None";
        Debug.Log($"{name} collided with {other.name}. Current crown owner: {currentOwnerName}");

        // 抢皇冠
        if (crown.currentOwner == other.transform && crown.currentOwner != transform)
        {
            crown.AttachToPlayer(transform);
            Debug.Log($"{name} stole the crown from {other.name}!");
        }
    }
}
