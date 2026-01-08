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

    [Header("Item Skills")]
    public ThunderSkill thunderSkill;

    void Start()
    {
        UpdateItemModel();
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        // ?? 右键使用道具
        if (currentItem != ItemType.None && Input.GetMouseButtonDown(1))
        {
            UseItem();
        }
    }

    // ======================
    // 拾取道具
    // ======================
    public void PickupItem(ItemType type)
    {
        if (currentItem != ItemType.None) return;

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
        if (thunderModel != null)
            thunderModel.SetActive(currentItem == ItemType.Thunder);
    }

    // ======================
    // 使用道具
    // ======================
    void UseItem()
    {
        switch (currentItem)
        {
            case ItemType.Thunder:
                if (thunderSkill != null)
                    thunderSkill.Activate();
                break;
        }

        // 使用后清空
        photonView.RPC(nameof(RPC_ClearItem), RpcTarget.All);
    }

    [PunRPC]
    void RPC_ClearItem()
    {
        currentItem = ItemType.None;
        UpdateItemModel();
    }
}
