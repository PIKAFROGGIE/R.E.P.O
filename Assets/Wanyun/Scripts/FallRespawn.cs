using UnityEngine;
using Photon.Pun;
using System.Collections;

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

    PhotonView pv;
    CharacterController cc;

    bool isRespawning = false;
    bool canDie = true;

    void Start()
    {
        pv = GetComponent<PhotonView>();
        cc = GetComponent<CharacterController>();
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

        yield return StartCoroutine(Fade(1f));
        yield return new WaitForSeconds(respawnDelay);

        Vector3 safePos = FindSafeRespawnPoint_Tag();
        cc.enabled = false;
        transform.position = safePos;
        transform.rotation = Quaternion.identity; // 需要保留朝向就删掉这一行
        cc.enabled = true;

        yield return new WaitForSeconds(invincibleAfterRespawn);
        yield return StartCoroutine(Fade(0f));

        canDie = true;
        isRespawning = false;
    }

    Vector3 FindSafeRespawnPoint_Tag()
    {
        Bounds b = respawnArea.bounds;

        for (int i = 0; i < maxTryCount; i++)
        {
            float x = Random.Range(b.min.x, b.max.x);
            float z = Random.Range(b.min.z, b.max.z);
            float y = b.max.y + groundOffset;

            Vector3 candidate = new Vector3(x, y, z);

            Collider[] hits = Physics.OverlapSphere(candidate, checkRadius, ~0, QueryTriggerInteraction.Ignore);
            bool hasPlayer = false;

            foreach (var hit in hits)
            {
                // 避免刷到其他玩家（也排除自己）
                if (hit.CompareTag("Player") && hit.gameObject != gameObject)
                {
                    hasPlayer = true;
                    break;
                }
            }

            if (!hasPlayer) return candidate;
        }

        return respawnArea.bounds.center + Vector3.up * groundOffset;
    }

    IEnumerator Fade(float target)
    {
        float start = fadeCanvas.alpha;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadeCanvas.alpha = Mathf.Lerp(start, target, t / fadeDuration);
            yield return null;
        }
        fadeCanvas.alpha = target;
    }
    public void ForceRespawn()
    {
        if (!pv.IsMine) return;
        if (isRespawning || !canDie) return;

        StartCoroutine(RespawnRoutine());
    }

}
