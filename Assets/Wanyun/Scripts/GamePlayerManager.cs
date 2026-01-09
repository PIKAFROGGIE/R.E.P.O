using Photon.Pun;
using UnityEngine;

public class GamePlayerManager : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private GameObject playerPrefab;

    [Header("Spawn Area")]
    [SerializeField] private BoxCollider respawnArea;

    void Start()
    {
        if (!PhotonNetwork.IsConnectedAndReady) return;
        if (playerPrefab == null)
        {
            Debug.LogError("Player Prefab not assigned!");
            return;
        }

        // 只有 TagObject 为 null 时才生成玩家
        if (PhotonNetwork.LocalPlayer.TagObject != null) return;

        Vector3 spawnPos = GetRandomSpawnPosition();

        GameObject player = PhotonNetwork.Instantiate(
            playerPrefab.name,
            spawnPos,
            Quaternion.identity
        );

        PhotonNetwork.LocalPlayer.TagObject = player;
    }

    Vector3 GetRandomSpawnPosition()
    {
        if (respawnArea == null)
        {
            return new Vector3(
                Random.Range(-10f, 10f),
                2f,
                Random.Range(-10f, 10f)
            );
        }

        Bounds b = respawnArea.bounds;
        float x = Random.Range(b.min.x, b.max.x);
        float z = Random.Range(b.min.z, b.max.z);
        float y = b.max.y + 0.5f;

        return new Vector3(x, y, z);
    }
}
