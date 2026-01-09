using UnityEngine;
using Photon.Pun;
using System.Collections;

public class ItemSpawner : MonoBehaviourPun
{
    [Header("Spawn Area")]
    public Vector3 areaSize = new Vector3(10f, 1f, 10f);

    [Header("Spawn Settings")]
    public GameObject[] itemPrefabs;
    public float spawnInterval = 5f;
    public int maxItemsInScene = 5;

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(SpawnRoutine());
        }
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (!PhotonNetwork.IsMasterClient) yield break;

            if (GameObject.FindGameObjectsWithTag("Item").Length >= maxItemsInScene)
                continue;

            SpawnItem();
        }
    }

    void SpawnItem()
    {
        Vector3 randomPos = transform.position + new Vector3(
            Random.Range(-areaSize.x / 2, areaSize.x / 2),
            Random.Range(-areaSize.y / 2, areaSize.y / 2),
            Random.Range(-areaSize.z / 2, areaSize.z / 2)
        );

        GameObject prefab = itemPrefabs[Random.Range(0, itemPrefabs.Length)];

        PhotonNetwork.Instantiate(
            prefab.name,
            randomPos,
            Quaternion.identity
        );

        Debug.Log("Spawn: " + prefab.name);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, areaSize);
    }
}
