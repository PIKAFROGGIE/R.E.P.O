using UnityEngine;

public class PlayerCrownHandler : MonoBehaviour
{
    private Crown crown;

    private void Start()
    {
        crown = FindObjectOfType<Crown>();
        if (crown == null)
        {
            Debug.LogError("Crown not found in scene!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Crown") && crown.currentOwner == null)
        {
            crown.AttachToPlayer(transform);
            Debug.Log($"{name} picked up the crown from the ground.");
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

        // 如果碰到的玩家是国王，而自己不是
        if (crown.currentOwner == other.transform && crown.currentOwner != transform)
        {
            crown.AttachToPlayer(transform);
            Debug.Log($"{name} stole the crown from {other.name}!");
        }
    }
}
