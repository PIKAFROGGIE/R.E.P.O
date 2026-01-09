using UnityEngine;
using Photon.Pun;

public class ItemPickup : MonoBehaviourPun
{
    public ItemType itemType;

    public float destroyY = -2f;
    void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (transform.position.y < destroyY)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerItemHandler handler = other.GetComponent<PlayerItemHandler>();
        PhotonView pv = other.GetComponent<PhotonView>();

        if (handler != null && pv != null && pv.IsMine)
        {
            bool picked = handler.PickupItem(itemType);

            if (picked)
            {
                photonView.RPC(nameof(RPC_Disable), RpcTarget.All);
            }
        }
    }

    [PunRPC]
    void RPC_Disable()
    {
        gameObject.SetActive(false);
    }
}
