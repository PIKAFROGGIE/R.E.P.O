using UnityEngine;
using Photon.Pun;

public enum ItemType
{
    None,
    Thunder,
    Plunger,
    Egg,
    Banana
}

public class PlayerItemHandler : MonoBehaviourPun
{
    [Header("Item State")]
    public ItemType currentItem = ItemType.None;

    [Header("Item Models (On Player)")]
    public GameObject thunderModel;
    public GameObject plungerModel;
    public GameObject eggModel;
    public GameObject bananaModel;

    [Header("Item Skills")]
    public ThunderSkill thunderSkill;
    public PlungerSkill plungerSkill;
    public EggSkill eggSkill;
    public BananaSkill bananaSkill;

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip pickupSFX;

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
    public bool PickupItem(ItemType type)
    {
        if (currentItem != ItemType.None)
            return false;

        if (photonView.IsMine)
        {
            PlayPickupSFX();
        }

        photonView.RPC(nameof(RPC_PickupItem), RpcTarget.All, type);
        return true;
    }
    void PlayPickupSFX()
    {
        if (audioSource != null && pickupSFX != null)
        {
            audioSource.PlayOneShot(pickupSFX);
        }
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

        if (plungerModel != null)
            plungerModel.SetActive(currentItem == ItemType.Plunger);

        if (eggModel != null)
            eggModel.SetActive(currentItem == ItemType.Egg);

        if (bananaModel != null)
            bananaModel.SetActive(currentItem == ItemType.Banana);

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
            case ItemType.Plunger:
                plungerSkill.Activate();
                break;
            case ItemType.Egg:
                eggSkill.Activate();
                break;
            case ItemType.Banana:
                bananaSkill.Activate();
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
