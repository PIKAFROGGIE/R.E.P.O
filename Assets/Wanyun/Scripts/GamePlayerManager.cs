using Photon.Pun;
using UnityEngine;

public class GamePlayerManager : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private GameObject playerPrefab;

    [Header("Spawn Area (Only for wanyun map)")]
    [SerializeField] private BoxCollider respawnArea;

    void Start()
    {
        if (!PhotonNetwork.IsConnectedAndReady) return;
        if (playerPrefab == null)
        {
            Debug.LogError("Player Prefab not assigned!");
            return;
        }

        // 防止重复生成
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
        // 如果没有设置 RespawnArea，兜底方案
        if (respawnArea == null)
        {
            Debug.LogWarning("RespawnArea not set, using default random.");
            return new Vector3(
                Random.Range(-10f, 10f),
                2f,
                Random.Range(-10f, 10f)
            );
        }

        Bounds b = respawnArea.bounds;

        float x = Random.Range(b.min.x, b.max.x);
        float z = Random.Range(b.min.z, b.max.z);
        float y = b.max.y + 0.5f; // 确保在地面上方

        return new Vector3(x, y, z);
    }
}
