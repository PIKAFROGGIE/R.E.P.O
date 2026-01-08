using UnityEngine;
using Photon.Pun;
using System.Collections;

public class FinishLineTrigger : MonoBehaviour
{
    [Header("Local Barrier")]
    public GameObject localBarrier;
    public float barrierDelay = 3f;

    private bool triggered = false;

    private void Start()
    {
        if (localBarrier != null)
            localBarrier.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        PhotonView pv = other.GetComponentInParent<PhotonView>();
        if (pv == null || !pv.IsMine) return;

        triggered = true;

        PlayerUIManager.Instance.ShowWinText();

        if (localBarrier != null)
            StartCoroutine(EnableBarrierAfterDelay());

        Debug.Log("Local player reached the finish line");
    }

    IEnumerator EnableBarrierAfterDelay()
    {
        yield return new WaitForSeconds(barrierDelay);

        if (localBarrier != null)
            localBarrier.SetActive(true);
    }
}
