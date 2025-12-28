using UnityEngine;
using Photon.Pun;
using System.Collections;

public class FallRespawn : MonoBehaviour
{
    [Header("Death Settings")]
    public float deathY = -20f;

    [Header("Respawn Area")]
    public BoxCollider respawnArea;
    public float groundOffset = 0.3f;

    [Header("Anti-Overlap Settings")]
    public float checkRadius = 0.6f;          // 检测玩家半径
    public LayerMask playerLayer;
    public int maxTryCount = 15;

    [Header("Fade Settings")]
    public CanvasGroup fadeCanvas;
    public float fadeDuration = 0.5f;
    public float respawnDelay = 0.2f;

    private PhotonView pv;
    private CharacterController cc;
    private bool isRespawning = false;

    void Start()
    {
        pv = GetComponent<PhotonView>();
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (!pv.IsMine) return;
        if (isRespawning) return;

        if (transform.position.y < deathY)
        {
            StartCoroutine(RespawnRoutine());
        }
    }

    IEnumerator RespawnRoutine()
    {
        isRespawning = true;

        // 1?? 黑屏
        yield return StartCoroutine(Fade(1f));

        yield return new WaitForSeconds(respawnDelay);

        // 2?? 找安全的随机点
        Vector3 safePos = FindSafeRespawnPoint();

        // 3?? 传送
        cc.enabled = false;
        transform.position = safePos;
        cc.enabled = true;

        // 4?? 淡入
        yield return StartCoroutine(Fade(0f));

        isRespawning = false;
    }

    Vector3 FindSafeRespawnPoint()
    {
        Bounds bounds = respawnArea.bounds;

        for (int i = 0; i < maxTryCount; i++)
        {
            float x = Random.Range(bounds.min.x, bounds.max.x);
            float z = Random.Range(bounds.min.z, bounds.max.z);
            float y = bounds.max.y + groundOffset;

            Vector3 candidate = new Vector3(x, y, z);

            bool hitPlayer = Physics.CheckSphere(
                candidate,
                checkRadius,
                playerLayer,
                QueryTriggerInteraction.Ignore
            );

            if (!hitPlayer)
            {
                return candidate;
            }
        }

        // 如果多次失败，返回区域中心（兜底）
        return respawnArea.bounds.center + Vector3.up * groundOffset;
    }

    IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = fadeCanvas.alpha;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadeCanvas.alpha = Mathf.Lerp(startAlpha, targetAlpha, t / fadeDuration);
            yield return null;
        }

        fadeCanvas.alpha = targetAlpha;
    }
}
