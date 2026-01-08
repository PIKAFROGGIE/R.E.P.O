using UnityEngine;
using System.Collections.Generic;

public enum SFXType { Count, Win, Spring, Hit }
public enum UIAudioType { Win, Click }
public enum BGMType { Menu, Race }

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource sfxSource;
    public AudioSource uiSource;

    public List<SFXEntry> sfxClips;
    public List<UIEntry> uiClips;

    Dictionary<SFXType, AudioClip> sfxDict;
    Dictionary<UIAudioType, AudioClip> uiDict;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        sfxDict = new Dictionary<SFXType, AudioClip>();
        uiDict = new Dictionary<UIAudioType, AudioClip>();

        foreach (var e in sfxClips) sfxDict[e.type] = e.clip;
        foreach (var e in uiClips) uiDict[e.type] = e.clip;
    }

    public void PlaySFX(SFXType type)
    {
        if (sfxDict.ContainsKey(type))
            sfxSource.PlayOneShot(sfxDict[type]);
    }

    public void PlayUI(UIAudioType type)
    {
        if (uiDict.ContainsKey(type))
            uiSource.PlayOneShot(uiDict[type]);
    }
}

[System.Serializable]
public class SFXEntry
{
    public SFXType type;
    public AudioClip clip;
}

[System.Serializable]
public class UIEntry
{
    public UIAudioType type;
    public AudioClip clip;
}
