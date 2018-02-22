using UnityEngine;
using System.Collections;

public delegate void UnityAction();

public class AudioManager : Singleton<AudioManager> 
{
    protected AudioManager() {} // guarantee this will be always a singleton only - can't use the constructor!

    public AudioSource BackgroundChannel;
    public AudioSource VoiceOverChannel;
    public AudioSource[] SFXChannels;

    public float fadeOutTime = 0.1f;
    public float fadeInTime = 0.1f;

    private ScreenFadeCallBack fadeInCallback; 
    private ScreenFadeCallBack fadeOutCallback;

    private AudioSource currentFadingInAudioSource;
    private AudioSource currentFadingOutAudioSource;

    public bool backgroundShuffle = false;
    public AudioClip[] backgroundClips;

    private void Awake()
    {
        if(backgroundShuffle)
        {
            PlayShuffleBackgroundClip();
        }
    }

    public void ActivateShuffleBackgroundMusic()
    {
        backgroundShuffle = true;
        PlayShuffleBackgroundClip();
    }

    private void PlayShuffleBackgroundClip()
    {
        if(!backgroundShuffle)
            return;

        SetBackgroundChannel(backgroundClips[Random.Range(0, backgroundClips.Length)], 0, PlayShuffleBackgroundClip);
    }

    public void SetVoiceOver(AudioClip track, UnityAction callback = null, float startDelay = 0.0f)
    {
        VoiceOverChannel.clip = track;
        VoiceOverChannel.PlayDelayed(startDelay);

        if(callback != null)
        {
			StartCoroutine(DoCallback(callback, track.length + startDelay));
        }
    }

    public void SetBackgroundChannel(AudioClip track, float startDelay = 0.0f, UnityAction callback = null)
    {
        BackgroundChannel.clip = track;
        BackgroundChannel.PlayDelayed(startDelay);

        if(callback != null)
        {
            StartCoroutine(DoCallback(callback, track.length + startDelay));
        }
    }

    public void EnableBGLoop()
    {
        BackgroundChannel.loop = true;
    }

    public void DisableBGLoop()
    {
        BackgroundChannel.loop = false;
    }
    
    public void SetSFXChannel(AudioClip track, UnityAction callback = null, float startDelay = 0.0f, int channelIndex=0)
    {
        SFXChannels[channelIndex].clip = track;
        SFXChannels[channelIndex].PlayDelayed(startDelay);

        if(callback != null)
        {
            StartCoroutine(DoCallback(callback, track.length + startDelay));
        }
    }

    public IEnumerator DoCallback(UnityAction callback, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (callback != null)
        {
            callback();
            callback = null;
        }
    }

    #region Fading methods

    public void FadeIn(AudioSource audioSource, float fadeTime, float delay=0.0f, float fadeInVolume = 0.5f, ScreenFadeCallBack callback=null)
    {
        fadeInCallback = callback;
        currentFadingInAudioSource = audioSource;

        iTween.ValueTo(currentFadingInAudioSource.gameObject, iTween.Hash (
            "from", 0.0f,
            "to", fadeInVolume,
            "time", fadeTime,
            "delay", delay,
            "easetype", "linear",
            "onComplete", "FadeInFinished",
            "onCompleteTarget", gameObject,
            "onUpdate", "OnFadeIn",
            "onUpdateTarget", gameObject
        ));
    }

    public void FadeOut(AudioSource audioSource, float fadeTime, float delay=0.0f, ScreenFadeCallBack callback=null)
    {
        fadeOutCallback = callback;
        currentFadingOutAudioSource = audioSource;

        iTween.ValueTo(currentFadingOutAudioSource.gameObject, iTween.Hash (
            "from", currentFadingOutAudioSource.volume,
            "to", 0.0f,
            "time", fadeTime,
            "delay", delay,
            "easetype", "linear",
            "onComplete", "FadeOutFinished",
            "onCompleteTarget", gameObject,
            "onUpdate", "OnFadeOut",
            "onUpdateTarget", gameObject
        ));
    }

    public void OnFadeIn(float value)
    {
        currentFadingInAudioSource.volume = value;
    }

    public void OnFadeOut(float value)
    {
        currentFadingOutAudioSource.volume = value;
    }

    public void FadeInFinished()
    {
        if(fadeInCallback != null)
            fadeInCallback();
    }

    public void FadeOutFinished()
    {
        if(fadeOutCallback != null)
            fadeOutCallback();
    }

    #endregion
}
