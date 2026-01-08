using UnityEngine;
using TMPro;

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
    public float stealCooldown = 1.0f;
    private static float stealCooldownTimer = 0f;

    // ===== TMP 3D 分数 =====
    public TextMeshPro scoreTMP;          // 在 Inspector 绑定 TMP 3D
    public Transform scoreBackground;     // 在 Inspector 绑定 Quad / Image 背景
    public Vector3 uiOffset = new Vector3(0, 2f, 0);

    [Header("指定分数面向的摄像机")]
    public Camera targetCamera;           // 可以在 Inspector 指定摄像机

    private void Start()
    {
        crown = FindObjectOfType<Crown>();
        if (crown == null)
            Debug.LogError("Crown not found in scene!");

        if (scoreTMP == null)
            Debug.LogError("ScoreTMP not assigned!");

        if (targetCamera == null)
            Debug.LogWarning("Target Camera not assigned. Defaulting to Camera.main");
    }

    private void Update()
    {
        if (crown == null) return;

        // 冷却计时
        if (stealCooldownTimer > 0f)
            stealCooldownTimer -= Time.deltaTime;

        HandleKingScore();
        HandleCrownPickUp();
        HandleCrownSteal();

        UpdateScoreUI();
        UpdateUIRotation();
    }

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

    private void HandleCrownSteal()
    {
        if (stealCooldownTimer > 0f) return;
        if (crown.currentOwner == null || crown.currentOwner == transform) return;

        float distance = Vector3.Distance(transform.position, crown.currentOwner.position);
        if (distance <= stealDistance)
        {
            Debug.Log($"{name} stole the crown from {crown.currentOwner.name}");
            crown.AttachToPlayer(transform);
            stealCooldownTimer = stealCooldown;
        }
    }

    // ===== 刷新分数文本 =====
    private void UpdateScoreUI()
    {
        if (scoreTMP != null)
            scoreTMP.text = score.ToString();
    }

    // ===== 分数和背景朝向指定摄像机 =====
    private void UpdateUIRotation()
    {
        Camera cam = targetCamera != null ? targetCamera : Camera.main;
        if (scoreTMP != null && cam != null)
        {
            scoreTMP.transform.forward = cam.transform.forward;
            if (scoreBackground != null)
                scoreBackground.forward = cam.transform.forward;
        }
    }
}
