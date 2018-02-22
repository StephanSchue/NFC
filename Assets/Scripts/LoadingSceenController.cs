using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadingSceenController : MonoBehaviour 
{
    #region variables

    public Animator cherryAnimator;

    private CanvasGroup canvasGroup;
    private CanvasGroup currentFadeCanvasGroup;

    public float fadeOutTime = 0.1f;
    public float fadeInTime = 0.1f;

    private ScreenFadeCallBack fadeInCallback; 
    private ScreenFadeCallBack fadeOutCallback;

    private AsyncOperation loadLevelAsyncOperation;

    private bool finishedLoading = false;
    public bool startLoading = false;

    private string previousScene = "StartScreen";
    private string sceneToBeLoaded = "Game";

    #endregion

    #region init methods

    protected void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        previousScene = LoadingScreenParamContainer.Instance.currentScene;
        sceneToBeLoaded = LoadingScreenParamContainer.Instance.sceneToBeLoaded;
    }

	/// <summary>
    /// Use this for initialization
    /// </summary>
	protected void Start()
    {
        EnableUI();
	}

    #endregion

    #region Fade methods

    public void EnableUI()
    {
        FadeIn(canvasGroup, fadeInTime, 0, StartLoadingGame);
    }

    public void DisableUI()
    {
        FadeOut(canvasGroup, fadeOutTime);
    }

    public void StartLoadingGame()
    {
        SceneManager.UnloadScene(previousScene);  

        if (sceneToBeLoaded != null)
        { 
            StartCoroutine(LoadGame(sceneToBeLoaded, false, 2.0f));
        }
    }

    public void FadeIn(CanvasGroup canvasGroup, float fadeTime, float delay=0.0f, ScreenFadeCallBack callback=null)
    {
        fadeInCallback = callback;
        currentFadeCanvasGroup = canvasGroup;

        iTween.ValueTo(currentFadeCanvasGroup.gameObject, iTween.Hash (
            "from", 0.0f,
            "to", 1.0f,
            "time", fadeTime,
            "delay", delay,
            "easetype", "linear",
            "onComplete", "FadeInFinished",
            "onCompleteTarget", gameObject,
            "onUpdate", "OnFade",
            "onUpdateTarget", gameObject
        ));
    }

    public void FadeOut(CanvasGroup canvasGroup, float fadeTime, float delay=0.0f, ScreenFadeCallBack callback=null)
    {
        fadeOutCallback = callback;
        currentFadeCanvasGroup = canvasGroup;

        iTween.ValueTo(currentFadeCanvasGroup.gameObject, iTween.Hash (
            "from", 1.0f,
            "to", 0.0f,
            "time", fadeOutTime,
            "delay", delay,
            "easetype", "linear",
            "onComplete", "FadeOutFinished",
            "onCompleteTarget", gameObject,
            "onUpdate", "OnFade",
            "onUpdateTarget", gameObject
        ));
    }

    public void OnFade(float value)
    {
        currentFadeCanvasGroup.alpha = value;
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

    private void UnloadStartScreen()
    {
        //SceneManager.UnloadScene("StartScreen");
    }

    private IEnumerator LoadGame(string sceneName, bool silent, float delay = 0.0f)
    {
        yield return new WaitForSeconds(delay);

        loadLevelAsyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        loadLevelAsyncOperation.allowSceneActivation = false;

        while (loadLevelAsyncOperation.progress < 0.9f) 
        {
            yield return null;
        }

        loadLevelAsyncOperation.allowSceneActivation = false;
        finishedLoading = true;

        if(!silent)
        {
            LoadIsFinished();
        }
    }

    private void LoadIsFinished()
    {
        loadLevelAsyncOperation.allowSceneActivation = true;
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneToBeLoaded));

        cherryAnimator.SetTrigger("PopIn");
        FadeOut(canvasGroup, fadeOutTime, 2.0f, DisabelCanvasGroup);
    }
    
    private void DisabelCanvasGroup()
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        SceneManager.UnloadScene("LoadingScreen");
    }
}
