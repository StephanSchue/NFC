using UnityEngine;
using System.Collections;

public delegate void IntroFinishCallback();

public class IntroController : MonoBehaviour 
{
    public GameObject ravenController;
    public Animator introSceneAC { get; set; }
    public Animator clearColorAC;
    public Animator habalogoAC;

    public AudioClip backgroundMusic;
    public AudioClip normalBackgroundMusic;

    public AudioClip introSFX;

    public float introDelay = 0.0f;

    public IntroFinishCallback OnIntroFinished;

    protected void Awake()
    {
        introSceneAC = GetComponent<Animator>();
    }

    public void StartIntro(IntroFinishCallback callback = null)
    {
        AudioManager.Instance.BackgroundChannel.volume = 0;
        AudioManager.Instance.FadeIn(AudioManager.Instance.BackgroundChannel, 14.0f);
        AudioManager.Instance.SetBackgroundChannel(backgroundMusic, 10.0f, FadeOutIntroMusic);
        AudioManager.Instance.DisableBGLoop();

        StartCoroutine(DoFunctionWithDelay(ActivatingIntro, introDelay));
        OnIntroFinished = callback;
    }

    public void FadeOutClearColor()
    {
        clearColorAC.enabled = true;
    }

    public void FadeOutHabaLogo()
    {
        habalogoAC.enabled = true;
    }

    public void ActivatingIntro()
    {
        ravenController.SetActive(true);
        introSceneAC.enabled = true;
        AudioManager.Instance.SetSFXChannel(introSFX);
    }

    public void FadeOutIntroMusic()
    {
        AudioManager.Instance.SetBackgroundChannel(normalBackgroundMusic);
        AudioManager.Instance.EnableBGLoop();
    }

    public void FinishIntro()
    {
        ravenController.SetActive(false);
        introSceneAC.enabled = false;
        StopAllCoroutines();

        if(OnIntroFinished != null)
        {
            OnIntroFinished();
        }
    }

    private IEnumerator DoFunctionWithDelay(FruitHighlightCallback method, float delay)
    {
        yield return new WaitForSeconds(delay);
        method();
    }
}
