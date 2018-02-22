using UnityEngine;
using System.Collections;

public class MoleController : MonoBehaviour 
{
    public GameObject molebody;
    public GameObject molehill;
    public Collider2D molebodyCollider;
    private SpriteRenderer[] moleRenderers;

    public float appearTime = 1.0f;
    public float appearDelayTime = 0.0f;
    public iTween.EaseType appearEaseType = iTween.EaseType.linear;

    public float minMoleStayTime = 3.0f; 
    public float maxMoleStayTime = 5.0f;

    public Vector3 moleHidePosition = Vector3.zero;
    public Vector3 moleShowPosition = Vector3.zero;

    public ParticleSystem dirtParticleSystem;
    public AudioClip hillAppears;
    public AudioClip shovelingSound;
    public AudioClip moleAppears;
    public AudioClip moleLeaves;

    private Animator animator;
    private UnityAction animationCompleteCallback;

    public bool isAppearing { get; private set; }
    public bool isDigging { get; set; }
    public bool appeared { get; private set; }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        moleRenderers = GetComponentsInChildren<SpriteRenderer>();
        appeared = false;
        isAppearing = false;
        isDigging = false;
    }

    public void Appear()
    {
        Appear(null);
    }

    public void Appear(UnityAction callback)
    {
        if(isAppearing)
            return;

        isAppearing = true;
        molebody.transform.localPosition = moleHidePosition;
        animationCompleteCallback = callback;

        iTween.MoveTo(molebody, iTween.Hash(
            "position", transform.position + moleShowPosition,
            "time", appearTime,
            "delay", appearDelayTime,
            "easetype", appearEaseType,
            "onComplete", "Idle",
            "onCompleteTarget", gameObject
        ));

        StartParticleDirt(appearTime);

        animator.SetTrigger("Appear");
        PlayShovelingSound(PlayMoleAppears);
    }

    public void Idle()
    {
        PlayMoleAppears();
        appeared = true;
    }

    public void Dig(UnityAction callback)
    {
        if(isDigging)
            return;

        isDigging = true;
        iTween.MoveTo(molebody, iTween.Hash(
            "position", transform.position + moleHidePosition,
            "time", appearTime,
            "delay", appearDelayTime,
            "easetype", appearEaseType,
            "onComplete", "CompleteDigging",
            "onCompleteTarget", gameObject
        ));

        StartParticleDirt(appearTime);

        animationCompleteCallback = callback;
        animator.SetTrigger("Dig");


        PlayMoleLeaves(PlayShovelingSound);    
        appeared = false;
    }

    public void CompleteDigging()
    {
        
    }

    public void SetSortingLayer(string sortinglayerName)
    {
        for (int i = 0; i < moleRenderers.Length; i++)
        {
            moleRenderers[i].sortingLayerName = sortinglayerName;
        }

        dirtParticleSystem.GetComponent<ParticleSystemRenderer>().sortingLayerName = sortinglayerName;
    }

    public void ShowSprites()
    {
        for (int i = 0; i < moleRenderers.Length; i++)
        {
            moleRenderers[i].enabled = true;
        }
            
        PlayHillAppears(null);

        isDigging = false;
        isAppearing = false;
    }

    public void HideSprites()
    {
        for (int i = 0; i < moleRenderers.Length; i++)
        {
            moleRenderers[i].enabled = true;
        }
    }

    public void AnimationComplete()
    {
        if(animationCompleteCallback != null)
        {
            animationCompleteCallback();
        }
    }

    public void StartParticleDirt(float duration)
    {
        ParticleSystem partSys = ParticleSystem.Instantiate(dirtParticleSystem, transform.position, transform.rotation) as ParticleSystem;
        StartCoroutine(ParticleEffectRunning(partSys, duration));
        Destroy(partSys.gameObject, 10.0f);
    }

    private IEnumerator ParticleEffectRunning(ParticleSystem particleSystem, float duration)
    {
        particleSystem.Play();

        yield return new WaitForSeconds(duration);

        if(particleSystem != null)
            particleSystem.Stop();
    }

    private void ClearParticles()
    {
        //dirtParticleSystem.Clear();
    }

    #region SFX

    public void PlayHillAppears(UnityAction callback)
    {
        if(AudioManager.Instance.SFXChannels[4].isPlaying)
            return;
        
        AudioManager.Instance.SetSFXChannel(hillAppears, callback , 0, 4);
    }

    public void PlayShovelingSound()
    {
        PlayShovelingSound(null);
    }

    public void PlayShovelingSound(UnityAction callback)
    {
        if(AudioManager.Instance.SFXChannels[4].isPlaying)
            return;
        
        AudioManager.Instance.SetSFXChannel(shovelingSound, callback, 0, 4);
    }

    public void PlayMoleAppears()
    {
        PlayMoleAppears(null);
    }

    public void PlayMoleAppears(UnityAction callback)
    {
        if(AudioManager.Instance.SFXChannels[4].isPlaying)
            return;
        
        AudioManager.Instance.SetSFXChannel(moleAppears, callback, 0, 4);
    }

    public void PlayMoleLeaves(UnityAction callback)
    {
        if(AudioManager.Instance.SFXChannels[4].isPlaying)
            return;
        
        AudioManager.Instance.SetSFXChannel(moleLeaves, callback, 0, 4);
    }

    #endregion


}
