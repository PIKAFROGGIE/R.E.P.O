using UnityEngine;
using Photon.Pun;

public class FinishLineTrigger : MonoBehaviour
{
    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        PhotonView pv = other.GetComponent<PhotonView>();
        if (pv != null && !pv.IsMine) return;

        triggered = true;

        // ֪ͨ GameEndManager
        GameEndManager.Instance.OnPlayerReachedFinish();
        GameEndManager.Instance.ShowWinText();

        Debug.Log("Player reached the finish line!");

    }
}
