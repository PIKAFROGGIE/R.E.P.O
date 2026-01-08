using UnityEngine;
using TMPro;
using ExitGames.Client.Photon;
using Photon.Pun;

public class PlayerCrownHandler : MonoBehaviourPun
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
    public TextMeshPro scoreTMP;
    public Transform scoreBackground;

    [Header("指定分数面向的摄像机")]
    public Camera targetCamera;

    private void Start()
    {
        crown = FindObjectOfType<Crown>();
        if (crown == null)
            Debug.LogError("Crown not found in scene!");

        if (scoreTMP == null)
            Debug.LogError("ScoreTMP not assigned!");

        if (targetCamera == null)
            Debug.LogWarning("Target Camera not assigned. Defaulting to Camera.main");

        // 初始化分数并同步到自己的 Photon Player
        score = 0;
        SyncScoreToPhoton();
    }

    private void Update()
    {
        if (crown == null) return;

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

                SyncScoreToPhoton(); // ✅ 每秒同步自己的分数

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

    private void UpdateScoreUI()
    {
        if (scoreTMP != null)
            scoreTMP.text = score.ToString();
    }

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

    public void SyncScoreToPhoton()
    {
        if (photonView == null) return;

        Hashtable props = new Hashtable
        {
            { "score", score }
        };

        // ⚡ 关键改动：使用 photonView.Owner 而不是 LocalPlayer
        photonView.Owner.SetCustomProperties(props);

        // TagObject 绑定到自己的 GameObject
        photonView.Owner.TagObject = this.gameObject;
    }
}
