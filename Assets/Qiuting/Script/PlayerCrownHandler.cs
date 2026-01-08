using UnityEngine;

public class PlayerCrownHandler : MonoBehaviour
{
    private Crown crown;

    // ===== 计分相关 =====
    public int score = 0;
    private float scoreTimer = 0f;
    private bool isKing = false;

    // ===== 距离检测 =====
    public float pickUpDistance = 1.5f;
    public float stealDistance = 1.5f;

    // ===== 抢夺冷却 =====
    // ===== 抢夺冷却（全局）=====
    public float stealCooldown = 1.0f;
    private static float stealCooldownTimer = 0f;


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

        // 冷却计时
        if (stealCooldownTimer > 0f)
        {
            stealCooldownTimer -= Time.deltaTime;
        }

        HandleKingScore();
        HandleCrownPickUp();
        HandleCrownSteal();
    }

    // =========================
    // 国王每秒加分
    // =========================
    private void HandleKingScore()
    {
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
        else if (isKing)
        {
            isKing = false;
            scoreTimer = 0f;
            Debug.Log($"{name} is no longer king.");
        }
    }

    // =========================
    // 捡地上皇冠
    // =========================
    private void HandleCrownPickUp()
    {
        if (crown.currentOwner != null) return;

        float distance = Vector3.Distance(transform.position, crown.transform.position);

        if (distance <= pickUpDistance)
        {
            crown.AttachToPlayer(transform);
            stealCooldownTimer = stealCooldown;
            Debug.Log($"{name} picked up the crown from the ground.");
        }
    }

    // =========================
    // 抢皇冠（关键修复在这里）
    // =========================
    private void HandleCrownSteal()
    {
        // 全局冷却
        if (stealCooldownTimer > 0f) return;

        if (crown.currentOwner == null) return;
        if (crown.currentOwner == transform) return; // 国王本人禁止抢

        float distance = Vector3.Distance(
            transform.position,
            crown.currentOwner.position
        );

        if (distance <= stealDistance)
        {
            Debug.Log($"{name} stole the crown from {crown.currentOwner.name}");
            crown.AttachToPlayer(transform);

            // 🔒 全局锁
            stealCooldownTimer = stealCooldown;
        }
    }

}
