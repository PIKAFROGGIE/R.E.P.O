using System.Collections;
using UnityEngine;

public enum BossState
{
    BackTurned,   // 绿灯
    Warning,      // 跳跃预兆
    Turning,      // 转身 + 判定开始
    FacingPlayer  // 红灯
}

public class BossController : MonoBehaviour
{
    public BossState state;

    [Header("Timing")]
    public Vector2 greenTime = new Vector2(5f, 10f);
    public float warningDuration = 1.0f;
    public float turnDuration = 1.0f;      // 与转身动画时长一致
    public Vector2 redTime = new Vector2(2f, 4f);

    [Header("Detection")]
    public bool isDetecting = false;

    [Header("Animator")]
    public Animator animator;

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip bossClip;
    public AudioClip warningSFX;   // 转身警告音

    private void Start()
    {
        PlayBGM();
        StartCoroutine(BossLoop());
    }

    IEnumerator BossLoop()
    {
        while (true)
        {
            // ===== Green Light =====
            state = BossState.BackTurned;
            isDetecting = false;

            animator.SetBool("IsFacingPlayer", false);
            PlayBGM();

            yield return new WaitForSeconds(Random.Range(greenTime.x, greenTime.y));

            // ===== Warning (Jump) =====
            state = BossState.Warning;
            PlayWarningSFX();
            animator.SetTrigger("Jump");

            yield return new WaitForSeconds(warningDuration);

            // ===== Music Stop + Detection ON =====
            PauseBGM();
            isDetecting = true;

            // ===== Turning =====
            state = BossState.Turning;
            animator.SetTrigger("Turn");

            yield return new WaitForSeconds(turnDuration);

            // ===== Facing Player =====
            state = BossState.FacingPlayer;
            animator.SetBool("IsFacingPlayer", true);

            yield return new WaitForSeconds(Random.Range(redTime.x, redTime.y));
        }
    }

    void PlayBGM()
    {
        if (audioSource == null || bossClip == null) return;

        if (!audioSource.isPlaying)
        {
            audioSource.clip = bossClip;
            audioSource.loop = true;

            if (audioSource.time > 0f)
            {
                audioSource.UnPause();   // 从暂停处继续
            }
            else
            {
                audioSource.Play();     // 第一次播放
            }
        }
    }

    void PauseBGM()
    {
        if (audioSource == null) return;

        audioSource.Pause();
    }

    // 转身开始的警告音
    public void PlayWarningSFX()
    {
        if (audioSource == null || warningSFX == null) return;

        audioSource.PlayOneShot(warningSFX);
    }

}
