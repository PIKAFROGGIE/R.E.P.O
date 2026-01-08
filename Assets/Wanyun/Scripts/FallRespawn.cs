using UnityEngine;
using Photon.Pun;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PhotonView))]
public class FallRespawn : MonoBehaviour
{
    [Header("Respawn")]
    public BoxCollider respawnArea;
    public float groundOffset = 0.3f;

    [Header("Anti-Overlap (Tag)")]
    public float checkRadius = 0.6f;
    public int maxTryCount = 15;

    [Header("Fade")]
    public CanvasGroup fadeCanvas;
    public float fadeDuration = 0.5f;
    public float respawnDelay = 0.2f;
    public float invincibleAfterRespawn = 0.4f;

    private PhotonView pv;
    private CharacterController cc;

    private bool isRespawning = false;
    private bool canDie = true;

    void Awake()
    {
        pv = GetComponent<PhotonView>();
        cc = GetComponent<CharacterController>();
    }

    void Start()
    {
        // 🔍 Prefab 无法拖引用 → 运行时查找 RespawnArea
        if (respawnArea == null)
        {
            GameObject area = GameObject.FindWithTag("RespawnArea");
            if (area != null)
            {
                respawnArea = area.GetComponent<BoxCollider>();
            }
            else
            {
                Debug.LogWarning("[FallRespawn] RespawnArea not found in scene!");
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!pv.IsMine) return;
        if (!canDie || isRespawning) return;

        if (other.CompareTag("KillZone"))
        {
            StartCoroutine(RespawnRoutine());
        }
    }

    IEnumerator RespawnRoutine()
    {
        isRespawning = true;
        canDie = false;

        // Fade In（黑屏）
        if (fadeCanvas != null)
            yield return StartCoroutine(Fade(1f));

        yield return new WaitForSeconds(respawnDelay);

        // 🧭 计算安全复活点
        Vector3 safePos = FindSafeRespawnPoint();
        cc.enabled = false;
        transform.position = safePos;
        cc.enabled = true;

        yield return new WaitForSeconds(invincibleAfterRespawn);

        // Fade Out（恢复画面）
        if (fadeCanvas != null)
            yield return StartCoroutine(Fade(0f));

        canDie = true;
        isRespawning = false;
    }

    Vector3 FindSafeRespawnPoint()
    {
        // 兜底：没有 RespawnArea 就原地上移
        if (respawnArea == null)
        {
            return transform.position + Vector3.up * 2f;
        }

        Bounds b = respawnArea.bounds;

        for (int i = 0; i < maxTryCount; i++)
        {
            float x = Random.Range(b.min.x, b.max.x);
            float z = Random.Range(b.min.z, b.max.z);
            float y = b.max.y + groundOffset;

            Vector3 candidate = new Vector3(x, y, z);

            // 🔒 防止刷到其他玩家
            Collider[] hits = Physics.OverlapSphere(
                candidate,
                checkRadius,
                ~0,
                QueryTriggerInteraction.Ignore
            );

            bool overlapPlayer = false;
            foreach (var hit in hits)
            {
                if (hit.CompareTag("Player") && hit.transform.root != transform)
                {
                    overlapPlayer = true;
                    break;
                }
            }

            if (!overlapPlayer)
                return candidate;
        }

        // 实在找不到 → 中心点兜底
        return respawnArea.bounds.center + Vector3.up * groundOffset;
    }

    IEnumerator Fade(float targetAlpha)
    {
        float start = fadeCanvas.alpha;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadeCanvas.alpha = Mathf.Lerp(start, targetAlpha, t / fadeDuration);
            yield return null;
        }

        fadeCanvas.alpha = targetAlpha;
    }

    /// <summary>
    /// 给外部（Boss / Trap / Debug）强制重生用
    /// </summary>
    public void ForceRespawn()
    {
        if (!pv.IsMine) return;
        if (isRespawning || !canDie) return;

        StartCoroutine(RespawnRoutine());
    }
}
