using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class PlayerNameDisplay : MonoBehaviourPun
{
    public Text nameText;

    [Header("Camera")]
    public Camera targetCamera;   // ← 手动指定的 Camera

    void Start()
    {
        if (nameText != null)
        {
            nameText.text = photonView.Owner.NickName;
        }

        if (targetCamera == null)
        {
            Debug.LogWarning("PlayerNameDisplay: targetCamera not assigned.");
        }
    }

    void LateUpdate()
    {
        if (targetCamera == null || nameText == null) return;

        // 始终面向指定摄像机
        nameText.transform.rotation =
            Quaternion.LookRotation(
                nameText.transform.position - targetCamera.transform.position
            );
    }
}
