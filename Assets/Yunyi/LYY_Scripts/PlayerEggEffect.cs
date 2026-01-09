using UnityEngine;
using Photon.Pun;
using System.Collections;

public class PlayerEggEffect : MonoBehaviourPun
{
    [Header("Egg UI")]
    public GameObject eggUI;
    public CanvasGroup canvasGroup;

    [Header("Timing")]
    public float fadeInTime = 0.2f;
    public float holdTime = 2.0f;
    public float fadeOutTime = 0.4f;

    Coroutine eggRoutine;

    [PunRPC]
    public void RPC_ShowEggEffect()
    {
        if (!photonView.IsMine) return;

        if (eggRoutine != null)
            StopCoroutine(eggRoutine);

        eggRoutine = StartCoroutine(EggRoutine());
    }

    IEnumerator EggRoutine()
    {
        eggUI.SetActive(true);
        canvasGroup.alpha = 0f;

        // Fade In
        float t = 0f;
        while (t < fadeInTime)
        {
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeInTime);
            t += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1f;

        // Hold
        yield return new WaitForSeconds(holdTime);

        // Fade Out
        t = 0f;
        while (t < fadeOutTime)
        {
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeOutTime);
            t += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0f;

        eggUI.SetActive(false);
    }
}
