using UnityEngine;

public class Crown : MonoBehaviour
{
    [HideInInspector]
    public Transform currentOwner;

    // 附着到玩家头顶
    public void AttachToPlayer(Transform player)
    {
        if (player == null)
        {
            Debug.LogError("AttachToPlayer: player is null!");
            return;
        }

        currentOwner = player;

        // 找玩家头顶挂点
        Transform headPoint = player.Find("HeadPoint");
        if (headPoint == null)
        {
            Debug.LogError("HeadPoint not found on player: " + player.name);
            return;
        }

        transform.SetParent(headPoint, false);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        Debug.Log($"{player.name} now has the crown!");
    }

    // 掉落皇冠到地面
    public void Detach()
    {
        currentOwner = null;
        transform.SetParent(null);
    }
}
