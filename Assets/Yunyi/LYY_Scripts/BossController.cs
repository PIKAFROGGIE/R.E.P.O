using System.Collections;
using UnityEngine;
using Photon.Pun;

public enum BossState
{
    BackTurned,
    Warning,
    Turning,
    FacingPlayer
}

public class BossController : MonoBehaviourPunCallbacks
{
    public BossState state;

    [Header("Timing")]
    public Vector2 greenTime = new Vector2(5f, 10f);
    public float warningDuration = 1.0f;
    public float turnDuration = 1.0f;
    public Vector2 redTime = new Vector2(2f, 4f);

    [Header("Detection")]
    public bool isDetecting = false;

    [Header("Animator")]
    public Animator animator;

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip bossClip;
    public AudioClip warningSFX;

    private void Start()
    {
        // 只让 Master 控制 Boss
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(BossLoop());
        }
    }

    IEnumerator BossLoop()
    {
        while (true)
        {
            photonView.RPC(nameof(RPC_GreenLight), RpcTarget.All);
            yield return new WaitForSeconds(Random.Range(greenTime.x, greenTime.y));

            photonView.RPC(nameof(RPC_Warning), RpcTarget.All);
            yield return new WaitForSeconds(warningDuration);

            photonView.RPC(nameof(RPC_StartTurning), RpcTarget.All);
            yield return new WaitForSeconds(turnDuration);

            photonView.RPC(nameof(RPC_FacingPlayer), RpcTarget.All);
            yield return new WaitForSeconds(Random.Range(redTime.x, redTime.y));
        }
    }

    // =========================
    // RPCs（所有客户端执行）
    // =========================

    [PunRPC]
    void RPC_GreenLight()
    {
        state = BossState.BackTurned;
        isDetecting = false;

        animator.SetBool("IsFacingPlayer", false);
        PlayBGM();
    }

    [PunRPC]
    void RPC_Warning()
    {
        state = BossState.Warning;

        PlayWarningSFX();
        animator.SetTrigger("Jump");
    }

    [PunRPC]
    void RPC_StartTurning()
    {
        PauseBGM();
        isDetecting = true;

        state = BossState.Turning;
        animator.SetTrigger("Turn");
    }

    [PunRPC]
    void RPC_FacingPlayer()
    {
        state = BossState.FacingPlayer;
        animator.SetBool("IsFacingPlayer", true);
    }

    // =========================
    // Sound
    // =========================

    void PlayBGM()
    {
        if (audioSource == null || bossClip == null) return;

        audioSource.clip = bossClip;
        audioSource.loop = true;

        if (!audioSource.isPlaying)
        {
            if (audioSource.time > 0f)
                audioSource.UnPause();
            else
                audioSource.Play();
        }
    }

    void PauseBGM()
    {
        if (audioSource == null) return;
        audioSource.Pause();
    }

    public void PlayWarningSFX()
    {
        if (audioSource == null || warningSFX == null) return;
        audioSource.PlayOneShot(warningSFX);
    }
}
