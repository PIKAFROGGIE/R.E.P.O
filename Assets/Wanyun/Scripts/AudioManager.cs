using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;
    public AudioSource uiSource;

    [Header("Volume")]
    [Range(0f, 1f)] public float bgmVolume = 0.8f;
    [Range(0f, 1f)] public float sfxVolume = 1f;
    [Range(0f, 1f)] public float uiVolume = 1f;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        ApplyVolume();
    }

    void ApplyVolume()
    {
        if (bgmSource) bgmSource.volume = bgmVolume;
        if (sfxSource) sfxSource.volume = sfxVolume;
        if (uiSource) uiSource.volume = uiVolume;
    }

    public void PlayBGM(AudioClip clip)
    {
        if (clip == null || bgmSource == null) return;

        if (bgmSource.clip == clip && bgmSource.isPlaying)
            return;

        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        if (bgmSource)
            bgmSource.Stop();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;

        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    public void PlayUI(AudioClip clip)
    {
        if (clip == null || uiSource == null) return;

        uiSource.PlayOneShot(clip, uiVolume);
    }
}
