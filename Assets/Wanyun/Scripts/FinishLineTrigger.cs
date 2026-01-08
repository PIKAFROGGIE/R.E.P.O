using UnityEngine;
using Photon.Pun;

public class FinishLineTrigger : MonoBehaviour
{
    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        PhotonView pv = other.GetComponentInParent<PhotonView>();
        if (pv == null || !pv.IsMine) return;

        PlayerUIManager ui = pv.GetComponent<PlayerUIManager>();
        if (ui == null)
        {
            Debug.LogError("PlayerUIManager not found on local player!");
            return;
        }

        triggered = true;

        GameEndManager.Instance.OnPlayerReachedFinish(ui);

        Debug.Log("Local player reached the finish line!");
    }
}
