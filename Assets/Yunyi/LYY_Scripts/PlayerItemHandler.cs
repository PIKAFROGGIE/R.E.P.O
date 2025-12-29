using UnityEngine;
using Photon.Pun;

public enum ItemType
{
    None,
    Thunder
}

public class PlayerItemHandler : MonoBehaviourPun
{
    [Header("Item State")]
    public ItemType currentItem = ItemType.None;

    [Header("Item Models (On Player)")]
    public GameObject thunderModel;

    void Update()
    {
        if (!photonView.IsMine) return;

        if (currentItem != ItemType.None && Input.GetMouseButtonDown(0))
        {
            UseItem();
        }
    }

    // ======================
    // 捡道具
    // ======================
    public void PickupItem(ItemType type)
    {
        if (currentItem != ItemType.None) return; // 已有道具，不能再捡

        photonView.RPC(nameof(RPC_PickupItem), RpcTarget.All, type);
    }

    [PunRPC]
    void RPC_PickupItem(ItemType type)
    {
        currentItem = type;
        UpdateItemModel();
    }

    void UpdateItemModel()
    {
        thunderModel.SetActive(currentItem == ItemType.Thunder);
    }

    // ======================
    // 使用道具
    // ======================
    void UseItem()
    {
        photonView.RPC(nameof(RPC_UseItem), RpcTarget.All, currentItem);
    }

    [PunRPC]
    void RPC_UseItem(ItemType type)
    {
        switch (type)
        {
            case ItemType.Thunder:
                Debug.Log("Use Thunder");
                break;
        }

        currentItem = ItemType.None;
        UpdateItemModel();
    }
}
