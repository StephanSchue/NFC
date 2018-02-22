using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UI_Controller_StartScreen : MonoBehaviour 
{
	public StartscreenController startScreenController;
	public RewardGardenController rewardGardenController;

    public CanvasGroup SeedPanel;
    public CanvasGroup GeneralUI;
    public CanvasGroup LogoCanvasGroup;

    public CanvasGroup SettingsButtonCanvasGroup;
    public CanvasGroup MoreAppsButtonCanvasGroup;

    public Transform SettingsButton_Anchor;
    public Transform MoreAppsButton_Anchor;

	public Image Blocker;

	public Button SettingsButton;
    
	public Button MoreAppsButton;
	public GameObject MoreAppsPanel;

	public GameObject Logo;
	public Button2D StartButton;

	public CanvasGroup fadeScreen;

	public float fadeOutTime = 0.1f;
	public float fadeInTime = 0.1f;

    public float generalFadeOutTime = 0.1f;
    public float generalFadeInTime = 0.1f;

	private ScreenFadeCallBack fadeInCallback; 
	private ScreenFadeCallBack fadeOutCallback;

    public Dropdown languageDropdown;

    public CanvasGroup mainCanvas;
    public CanvasGroup mainUICanvas;

    public CanvasGroup SettingsPanel;
    public CanvasGroup CreditsPanel;

    private CanvasGroup currentFadeInCanvasGroup;
    private CanvasGroup currentFadeOutCanvasGroup;

    public AudioClip buttonClickAudioClip;
    public CreditsScroll creditsScroll;

    public List<CanvasGroup> fadeGroups = new List<CanvasGroup>();

    [Header("Logo Shaking")]
    public ParticleSystem leafParticleSystem;
    public Vector3 logoLeafShakeIntensity = Vector3.one;
    public float logoLeafShakeTime = 0.1f;
    public Vector3 logoLeafParticleSystemOffset = Vector3.zero;
    public AudioClip logoShakingSFX;

	[Header("Seed Mode")]
	public Button SeedPocket;
	public UI_CountDisplay SeedCountLabel;
	public Text SeedCollected;

    public Image HintHand;
    public GameObject HintHandStartPosition;
    public GameObject HintHandEndPosition;
    public float HintHandSpeed = 1.0f;
    public iTween.EaseType HintHandEaseType = iTween.EaseType.linear;

	private bool status = true;
    private bool languageSet = false;

    public bool inPopupWindow { get; private set; }

	#region General UI functions

	protected void Awake ()
	{
        inPopupWindow = false;

        //WebPromoSDK.InitPromo("1135867247");
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            #if UNITY_IOS
            xCodeBridge.Mute();
            #endif
        }
	}

	protected void Start()
	{
		startScreenController.OnSeedCountChange += UpdateSeedPocket;
        languageDropdown.onValueChanged.AddListener(delegate {
            ChangeLanguage(languageDropdown);
        });
	}
        
    protected void Destroy() 
    {
        languageDropdown.onValueChanged.RemoveAllListeners();
    }

	public void HidePopupWindows()
	{
		HideSettingsPanel();
		HideMoreAppsPanel();
        HideCreditScreen();

        FadeIn(GeneralUI, generalFadeInTime, 0.0f);
        //ActivateFadeInCanvasGroupControls();

        inPopupWindow = false;
	}

	public void ShowUI()
	{
		SettingsButton.gameObject.SetActive(true);
		MoreAppsButton.gameObject.SetActive(true);

		//StartButton.gameObject.SetActive(true);
		Logo.gameObject.SetActive(true);
	}

    public void ShowLogo()
    {
        FadeIn(LogoCanvasGroup, generalFadeInTime, 0.0f);
    }

    public void HideLogo()
    {
        FadeOut(LogoCanvasGroup, generalFadeOutTime, 0.0f, DeactivateFadeOutCanvasGroupControls);
    }

    public void ShowButtons(float delay)
    {
        iTween.MoveTo(SettingsButton.gameObject, iTween.Hash(
            "position", SettingsButton_Anchor.position,
            "time", 0.5f,
            "delay", 2.0f,
            "easetype", iTween.EaseType.easeOutQuad
        ));

        iTween.MoveTo(MoreAppsButton.gameObject, iTween.Hash(
            "position", MoreAppsButton_Anchor.position,
            "time", 0.5f,
            "delay", 2.0f,
            "easetype", iTween.EaseType.easeOutQuad
        ));

        //FadeIn(SettingsButtonCanvasGroup, generalFadeInTime, delay);
        //FadeIn(MoreAppsButtonCanvasGroup, generalFadeInTime, delay);
    }

	public void HideUI()
	{
		SettingsButton.gameObject.SetActive(false);
		MoreAppsButton.gameObject.SetActive(false);

		//StartButton.gameObject.SetActive(false);
		Logo.gameObject.SetActive(false);
	}

	public void ToggleUI()
	{
		HidePopupWindows();

		if(status) 
		{
			HideUI();
		} 
		else 
		{
			ShowUI();
		}

		status = !status;
	}

	#endregion

	
	#region Blocker functions

	public void EnableBlocker()
	{
		Blocker.enabled = true;
	}

	public void DisableBlocker()
	{
		Blocker.enabled = false;
	}

	#endregion

	
	#region Settings Panel functions

	public void ShowSettingsPanel()
	{
        inPopupWindow = true;
        PlayButtonClickSound();

        HideSeedPlantTutorial();

		StartButton.enabled = false;

        SeedPocket.gameObject.SetActive(false);
        
        FadeOut(GeneralUI, generalFadeOutTime, 0.0f);
        DeactivateFadeOutCanvasGroupControls();

        FadeIn(SettingsPanel, generalFadeInTime, 0.0f);
        //ActivateFadeInCanvasGroupControls();
	}

    public void ShowSeedPocket()
    {
        SeedPocket.gameObject.SetActive(true);
    }

    public void ShowSettingsPanelFinished()
    {
        //ActivateFadeInCanvasGroupControls();
    }

	public void HideSettingsPanel()
	{
        inPopupWindow = false;
        PlayButtonClickSound();
		StartButton.enabled = true;
        
        FadeOut(SettingsPanel, generalFadeOutTime, 0.0f, HideSettingsPanelFinished);
	}

    public void HideSettingsPanelFinished()
    {
        DeactivateFadeOutCanvasGroupControls();
        SeedPocket.gameObject.SetActive(true);

        if(startScreenController.GetCountCollectedSeeds() > 0 && startScreenController.GetCountPlacedSeeds() == 0)
        {
            ShowSeedPlantTutorial();
        }
    }

	public void ToogleSettingsPanel()
	{
        if(SettingsPanel.alpha >= 1.0f) 
		{
			HideSettingsPanel();
		} 
		else 
		{
			ShowSettingsPanel();
		}
	}

    public void ActivateFadeInCanvasGroupControls()
    {
        if (currentFadeInCanvasGroup == null)
            return;

        currentFadeInCanvasGroup.interactable = true;
        currentFadeInCanvasGroup.blocksRaycasts = true;
    }

    public void ActivateFadeOutCanvasGroupControls()
    {
        if (currentFadeOutCanvasGroup == null)
            return;
        
        currentFadeOutCanvasGroup.interactable = true;
        currentFadeOutCanvasGroup.blocksRaycasts = true;
    }

    public void DeactivateFadeInCanvasGroupControls()
    {
        if (currentFadeInCanvasGroup == null)
            return;
        
        currentFadeInCanvasGroup.interactable = false;
        currentFadeInCanvasGroup.blocksRaycasts = false;
    }

    public void DeactivateFadeOutCanvasGroupControls()
    {
        if (currentFadeOutCanvasGroup == null)
            return;
        
        currentFadeOutCanvasGroup.interactable = false;
        currentFadeOutCanvasGroup.blocksRaycasts = false;
    }

	#endregion

	
	#region MoreApps Panel functions

	public void ShowMoreAppsPanel()
	{
		//StartButton.enabled = false;
        //mainCanvas.interactable = false;
        //mainCanvas.blocksRaycasts = false;

        PlayButtonClickSound();
        //startScreenController.ShowMoreApps();

        //inPopupWindow = true;
	}

	public void HideMoreAppsPanel()
	{
        //inPopupWindow = false;

        //StartButton.enabled = true;
        //mainCanvas.interactable = true;
        //mainCanvas.blocksRaycasts = true;

        Destroy(GameObject.Find("GateCanvas(Clone)"));

        PlayButtonClickSound();
        // WebPromoSDK has no Hide Option
	}

	public void ToogleMoreAppsPanel()
	{
		if(MoreAppsPanel.activeSelf) 
		{
			HideMoreAppsPanel();
		} 
		else 
		{
            ShowMoreAppsPanel();
		}
	}

	#endregion

	#region SeedMode functions

    /// <summary>
    /// Starts the place seed.
    /// </summary>
	public void StartPlaceSeed()
	{
		if(!startScreenController.IsSeedInThePocket())
			return;

        HideSeedPlantTutorial();

		// Hide UI
        DisableMainUI(0.0f, ShowRewardMenu);
        
		// Start PlacementMode
		rewardGardenController.StartPlaceSeed();
	}

    /// <summary>
    /// Shows the reward menu.
    /// </summary>
    private void ShowRewardMenu()
    {
        inPopupWindow = true;
        FadeIn(SeedPanel, generalFadeInTime, 0.0f);
    }

    /// <summary>
    /// Updates the seed pocket.
    /// </summary>
    /// <param name="count">Count.</param>
	public void UpdateSeedPocket(int count)
	{
		SeedPocket.interactable = count > 0 ? true : false;

		if(!SeedPocket.interactable)
			rewardGardenController.StopPlaceSeed();

        SeedCountLabel.SetCount(count);
		SetSeedPlacedCount();

        if(count > 0)
        {
            ShowSeedPocketLabel();
        }
        else
        {
            HideSeedPocketLabel();
        }
	}

    public void ShowSeedPocketLabel()
    {
        SeedCountLabel.ShowContent(true);
    }

    public void HideSeedPocketLabel()
    {
        SeedCountLabel.ShowContent(false);
    }


    /// <summary>
    /// Sets the seed placed count.
    /// </summary>
	public void SetSeedPlacedCount()
	{
		SeedCollected.text = startScreenController.GetCountCollectedSeeds().ToString();
	}

    /// <summary>
    /// Leaves the reward garden.
    /// </summary>
    public void LeaveRewardGarden()
    {
        FadeOut(SeedPanel, generalFadeOutTime, 0.0f, FadeInTheUI);
        rewardGardenController.StopPlaceSeed();

        PlayButtonClickSound();
    }

    /// <summary>
    /// Deactivate the SeedPanel & Fade in the MainCanvas
    /// </summary>
    public void FadeInTheUI()
    {
        DeactivateFadeOutCanvasGroupControls();
        EnableMainUI();
        //ActivateFadeInCanvasGroupControls();
        creditsScroll.ResetPosition();
    }


	#endregion

    #region Credits

    public void ShowCreditScreen()
    {
        inPopupWindow = true;
        DisableMainUI(0.0f);
        FadeIn(CreditsPanel, generalFadeInTime, 0.0f);
        PlayButtonClickSound();
        creditsScroll.StartScroll();
    }

    public void HideCreditScreen()
    {
        inPopupWindow = false;
        FadeOut(CreditsPanel, generalFadeOutTime, 0.0f, FadeInTheUI);
        PlayButtonClickSound();
    }

    #endregion

	#region FadeScreen functions

    /*
	public void FadeIn(ScreenFadeCallBack callback=null)
	{
		fadeInCallback = callback;

		iTween.ValueTo(fadeScreen.gameObject, iTween.Hash (
			"from", 1.0f,
			"to", 0.0f,
			"time", fadeInTime,
			"easetype", "linear",
			"onComplete", "FadeInFinished",
			"onCompleteTarget", gameObject,
			"onUpdate", "OnFadeIn",
			"onUpdateTarget", gameObject
		));
	}

	public void FadeInFinished()
	{
		if(fadeInCallback != null)
			fadeInCallback();
	}

	public void FadeOut(ScreenFadeCallBack callback=null)
	{
		fadeOutCallback = callback;

		iTween.ValueTo(fadeScreen.gameObject, iTween.Hash (
			"from", 0.0f,
			"to", 1.0f,
			"time", fadeOutTime,
			"easetype", "linear",
			"onComplete", "FadeOutFinished",
			"onCompleteTarget", gameObject,
			"onUpdate", "OnFadeOut",
			"onUpdateTarget", gameObject
		));
	}
    */   

    /*
	public void FadeOutFinished()
	{
		if(fadeOutCallback != null)
			fadeOutCallback();
	}
    */

	public void OnFadeIn(float value)
	{
		fadeScreen.alpha = value;
	}

    public void OnFadeOut(float value)
    {
        fadeScreen.alpha = value;
    }

    #endregion

    #region UICanvas

    public void EnableUI(float delay=0.5f, ScreenFadeCallBack callback=null)
    {
        FadeIn(mainCanvas, generalFadeInTime, delay, callback);
        ShowButtons(delay + generalFadeInTime);
    }

    public void DisableUI(float delay=0.5f, ScreenFadeCallBack callback=null)
    {
        FadeOut(mainCanvas, generalFadeOutTime, delay, callback);
    }

    public void EnableMainUI(float delay=0.5f, ScreenFadeCallBack callback=null)
    {
        FadeIn(mainUICanvas, generalFadeInTime, delay, callback);
    }

    public void DisableMainUI(float delay=0.5f, ScreenFadeCallBack callback=null)
    {
        FadeOut(mainUICanvas, generalFadeOutTime, delay, callback);
    }

    public void FadeIn(CanvasGroup canvasGroup, float fadeTime, float delay=0.0f, ScreenFadeCallBack callback=null)
    {
        if(canvasGroup.alpha >= 1.0f)
        {
            return;
        }

        //StartCoroutine(OnFading(canvasGroup, fadeTime, delay));
        StartCoroutine(DoCallback(callback, canvasGroup, true, delay + fadeTime));

        iTween.ValueTo(canvasGroup.gameObject, iTween.Hash (
            "from", 0.0f,
            "to", 1.0f,
            "time", fadeTime,
            "delay", delay,
            "easetype", "linear",
            "onComplete", "FadeInFinished",
            "onCompleteTarget", gameObject,
            "onUpdate", "Fade" + canvasGroup.name,
            "onUpdateTarget", gameObject
        ));

        Debug.Log("Fade" + canvasGroup.name);
    }

    public void FadeOut(CanvasGroup canvasGroup, float fadeTime, float delay=0.0f, ScreenFadeCallBack callback=null)
    { 
        if(canvasGroup.alpha <= 0.0f)
        {
            return;
        }

        //StartCoroutine(OnFading(canvasGroup, fadeTime, delay));
        StartCoroutine(DoCallback(callback, canvasGroup, false, delay + fadeTime));

        iTween.ValueTo(canvasGroup.gameObject, iTween.Hash (
            "from", 1.0f,
            "to", 0.0f,
            "time", fadeTime,
            "delay", delay,
            "easetype", "linear",
            "onComplete", "FadeOutFinished",
            "onCompleteTarget", gameObject,
            "onUpdate", "Fade" + canvasGroup.name,
            "onUpdateTarget", gameObject
        ));

        Debug.Log("Fade" + canvasGroup.name);
    }

    public IEnumerator OnFading(CanvasGroup canvasGroup, float time, float delay)
    {
        yield return new WaitForSeconds(delay);

        float destTime = Time.time + time;

        while(Time.time < destTime)
        {
            canvasGroup.alpha = Time.time / destTime;
            yield return new WaitForEndOfFrame();
        }
    }

    public void OnFadeInGeneral(float value)
    {
        currentFadeInCanvasGroup.alpha = value;
    }

    public void OnFadeOutGeneral(float value)
    {
        currentFadeOutCanvasGroup.alpha = value;
    }

	#endregion

    #region Language

    /// <summary>
    /// Changes the language.
    /// </summary>
    /// <param name="dropDownIndex">Drop down index.</param>
    public void ChangeLanguage(int dropDownIndex)
    {
        startScreenController.ChangeLanguage(dropDownIndex);
    }

    /// <summary>
    /// Changes the language.
    /// </summary>
    /// <param name="target">Target.</param>
    private void ChangeLanguage(Dropdown target) 
    {
        if(languageSet)
            PlayButtonClickSound();
        
        ChangeLanguage(target.value);
        languageSet = true;
    }
        
    /// <summary>
    /// Sets the index of the language.
    /// </summary>
    /// <param name="index">Index.</param>
    public void SetLanguageIndex(int index) 
    {
        languageDropdown.value = index;
        PlayButtonClickSound();
    }

    #endregion

    public void ClearGarden()
    {
        PlayButtonClickSound();
        startScreenController.ClearGarden();
    }

    public void PlayButtonClickSound()
    {
        AudioManager.Instance.SetSFXChannel(buttonClickAudioClip, null, 0, 1);
    }

    public IEnumerator DoCallback(ScreenFadeCallBack callback, CanvasGroup canvasGroup, bool activate, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (callback != null)
        {
            callback();
            callback = null;
        }

        if(canvasGroup != null)
        {
            canvasGroup.interactable = activate;
            canvasGroup.blocksRaycasts = activate;
        }
    }

    #region FadeMethods

    public void FadeUI_StartScreen(float value)
    {
        mainCanvas.alpha = value;
    }

    public void FadeGeneralUI(float value)
    {
        GeneralUI.alpha = value;
    }

    public void FadeSettingsPanel(float value)
    {
        SettingsPanel.alpha = value;
    }

    public void FadeLogo(float value)
    {
        LogoCanvasGroup.alpha = value;
    }

    public void FadeMainCanvas(float value)
    {
        mainUICanvas.alpha = value;
    }

    public void FadeCreditsPanel(float value)
    {
        CreditsPanel.alpha = value;
    }

    public void FadeBackToMainMenu(float value)
    {
        SeedPanel.alpha = value;
    }

    public void FadeButton_Settings(float value)
    {
        SettingsButtonCanvasGroup.alpha = value;
    }

    public void FadeButton_MoreApps(float value)
    {
        MoreAppsButtonCanvasGroup.alpha = value;
    }
        
    #endregion

    public void StartLogoAnimation()
    {
        //Debug.Log(screenPosition);
        ParticleSystem partSys = ParticleSystem.Instantiate(leafParticleSystem, transform.position, transform.rotation) as ParticleSystem;
        Destroy(partSys.gameObject, 10.0f);
       
        partSys.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
            Input.mousePosition.y, Camera.main.nearClipPlane + logoLeafParticleSystemOffset.z));

        iTween.ShakeRotation(Logo, iTween.Hash(
            "amount", logoLeafShakeIntensity,
            "time", logoLeafShakeTime,
            "delay", 0.0f,
            "onComplete", "ShakeFinished",
            "onCompleteTarget", gameObject
        ));

        AudioManager.Instance.SetSFXChannel(logoShakingSFX, null, 0, 2);
        partSys.Play();
    }

    public void FirstSession()
    {
        MoreAppsButton.gameObject.SetActive(false);
        //SettingsButton.gameObject.SetActive(false);
    }

    public void ShowSeedPlantTutorial()
    {
        HintHand.transform.position = HintHandStartPosition.transform.position;
        HintHand.enabled = true;

        iTween.MoveTo(HintHand.gameObject, iTween.Hash(
            "position", HintHandEndPosition.transform.position,
            "time", HintHandSpeed,
            "delay", 0.0f,
            "easetype", HintHandEaseType,
            "looptype", iTween.LoopType.pingPong
        ));
    }

    public void HideSeedPlantTutorial()
    {
        iTween.Stop(HintHand.gameObject);
        HintHand.enabled = false;
    }
}
