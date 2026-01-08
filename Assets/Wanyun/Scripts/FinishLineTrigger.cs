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

        triggered = true;

        GameEndManager.Instance.OnPlayerReachedFinish();

        Debug.Log("Local player reached the finish line!");
    }
}
