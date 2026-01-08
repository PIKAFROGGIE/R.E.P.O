using UnityEngine;
using Photon.Pun;

public class ItemPickup : MonoBehaviourPun
{
    public ItemType itemType;

    private void OnTriggerEnter(Collider other)
    {
        PlayerItemHandler handler = other.GetComponent<PlayerItemHandler>();
        PhotonView pv = other.GetComponent<PhotonView>();

        if (handler != null && pv != null && pv.IsMine)
        {
            handler.PickupItem(itemType);
            PhotonNetwork.Destroy(gameObject); // 所有人消失
        }
    }
}