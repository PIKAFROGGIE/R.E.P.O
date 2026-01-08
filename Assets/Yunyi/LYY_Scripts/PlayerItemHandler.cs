using UnityEngine;
using Photon.Pun;

public enum ItemType
{
    None,
    Thunder
}

public class PlayerItemHandler : MonoBehaviourPun
{
    public ItemType currentItem = ItemType.None;

    [Header("Thunder")]
    public ThunderItem thunderPrefab;
    public Transform thunderSpawnPoint;

    void Update()
    {
        if (!photonView.IsMine) return;

        // 右键使用
        if (Input.GetMouseButtonDown(1))
        {
            UseItem();
        }
    }

    public void PickupItem(ItemType item)
    {
        if (currentItem != ItemType.None) return;
        currentItem = item;
    }

    void UseItem()
    {
        if (currentItem == ItemType.None) return;

        switch (currentItem)
        {
            case ItemType.Thunder:
                photonView.RPC(nameof(RPC_UseThunder), RpcTarget.All);
                break;
        }

        currentItem = ItemType.None;
    }

    [PunRPC]
    void RPC_UseThunder()
    {
        ThunderItem thunder = Instantiate(
            thunderPrefab,
            thunderSpawnPoint.position,
            Quaternion.identity
        );

        // 把“使用者 ActorNumber”传进去
        thunder.Activate(photonView.OwnerActorNr);
    }
}

