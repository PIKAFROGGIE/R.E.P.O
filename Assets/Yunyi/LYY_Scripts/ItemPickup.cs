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

            // 所有人看到消失
            photonView.RPC(nameof(RPC_Disable), RpcTarget.All);
        }
    }

    [PunRPC]
    void RPC_Disable()
    {
        gameObject.SetActive(false);
    }
}
