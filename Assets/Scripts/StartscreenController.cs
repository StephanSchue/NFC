using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public delegate void SeedCountChangeCallback(int count);
public delegate void LoadSaveGameCompleteCallback();
public delegate void LoadClearCompleteCallback();

public class StartscreenController : MonoBehaviour, IStartscreenMessageTarget
{
	public Camera camera;
    public CameraController cameraParallaxController;
	public Transform positionOfGameCamera;
	public PlaceSeedController placeSeedController;
	public UI_Controller_StartScreen uiController;

    public RavenController ravenController;

	public bool preloadGame = true;
	private bool finishedLoading = false;
    public bool startLoading = false;

    public bool forceIntro = true;
    public bool introIsPlaying { get; private set; }

	private PlayerSaveGameController saveGameController;

	public SeedCountChangeCallback OnSeedCountChange;
	public LoadSaveGameCompleteCallback OnLoadSaveGameComplete;
	public LoadClearCompleteCallback OnClearGameComplete;

    public bool trackStartUps = true;
    public string levelName = "Game";

    public Transform landscape;
    public Transform aspect16to9;
    public Transform aspect4to3;
    public Transform aspect3to2;

    public IntroController introController;
    public Transform IntroLandscape;
    public Transform IntroFinalPosition;

    public AudioClip backgroundMusic;

	private AsyncOperation loadLevelAsyncOperation;

    public GameObject flowerContainer;

    public Transform ravenStartPosition;

	#region Initialize

	private void Awake()
	{
		saveGameController = new PlayerSaveGameController();
        introController.OnIntroFinished += IntroIsFinished;
        flowerContainer.SetActive(false);
	}

    private void OnDestroy()
    {
        introController.OnIntroFinished -= IntroIsFinished;
    }

	private void Start()
	{
		if(preloadGame) 
		{
			StartCoroutine(LoadGame(true));
		}

		LoadData();


        if(saveGameController.current.startUps == 0)
        {
            uiController.FirstSession();
        }

        if(forceIntro || saveGameController.current.showIntro)
        {
            camera.enabled = true;
            introController.StartIntro(IntroIsFinished);
            saveGameController.DisableIntro();
            introIsPlaying = true;
        }
        else
        {
            introIsPlaying = false;
            AudioManager.Instance.SetBackgroundChannel(backgroundMusic);
            AudioManager.Instance.EnableBGLoop();

            ravenController.gameObject.SetActive(true);
            ravenController.DoInvisible();

            AdjustCamera();
            StartCoroutine(DoFunctionWithDelay(ShowGameScreen, 0.1f));
        }
        
        if(trackStartUps)
        {
            IncreaseStartupCount();
        }
	}

	#endregion

    private IEnumerator DoFunctionWithDelay(FruitHighlightCallback method, float delay)
    {
        yield return new WaitForSeconds(delay);
        method();
    }

    public void ShowGameScreen()
    {
        introIsPlaying = false;
        flowerContainer.SetActive(true);
        camera.enabled = true;
        uiController.EnableUI(0.5f, ActivateUIControls);

        if(cameraParallaxController != null)
            cameraParallaxController.enabled = true;

        StartCoroutine(DoFunctionWithDelay(PositionRaven, 1.0f));

        ravenController.StartUp(AfterRavenStartUp);
        ravenController.DoVisible();
    }

    public void AfterRavenStartUp()
    {
        uiController.ShowLogo();
        ravenController.Idle(null);
    }

    private void ActivateUIControls()
    {
        uiController.ActivateFadeInCanvasGroupControls();

        if(GetCountUnplacedSeeds() > 0)
        {
            uiController.ShowSeedPocketLabel();
        }

        if(GetCountCollectedSeeds() > 0 && GetCountPlacedSeeds() == 0)
        {
            uiController.ShowSeedPlantTutorial();
        }

        uiController.ShowSeedPocket();
    }


    public void SkipIntro()
    {
        camera.enabled = false;
        introController.FinishIntro();

        AudioManager.Instance.SetBackgroundChannel(backgroundMusic);
        AudioManager.Instance.EnableBGLoop();

        ravenController.gameObject.SetActive(true);
        ravenController.DoInvisible();
        AdjustCamera();

        IntroIsFinished();
        StartCoroutine(DoFunctionWithDelay(EnableCamera, 0.1f));
    }

    public void EnableCamera()
    {
        camera.enabled = true;
    }

    public void DisableCamera()
    {
        camera.enabled = false;
    }

    public void IntroIsFinished()
    {
        introIsPlaying = false;
        flowerContainer.SetActive(true);
        uiController.EnableUI(0.5f, ActivateUIControls);

        if(cameraParallaxController != null)
            cameraParallaxController.enabled = true;

        StartCoroutine(DoFunctionWithDelay(PositionRaven, 1.0f));

        ravenController.gameObject.SetActive(true);

        ravenController.StartUp(AfterRavenStartUp);
        ravenController.DoVisible();
    }

    private void PositionRaven()
    {
        ravenController.transform.position = ravenStartPosition.position;
    }

	#region MessageSystem fuctions

	public void PressStart()
	{
        if(startLoading)
            return;

        startLoading = true;

		if(preloadGame)
		{
			LoadIsFinished();
		}
		else
		{
			finishedLoading = false;
			StartCoroutine(LoadGame(false));
		}
	}

    public void PressTutorial()
    {
        saveGameController.EnableTutorial();
        saveGameController.SaveData();

        PressStart();
    }

    public void AdjustCamera()
    {
        introController.introSceneAC.enabled = true;
        introController.introSceneAC.SetTrigger("End");

        //IntroLandscape.position = IntroFinalPosition.position;

        /*
        if(camera.aspect >= 1.7)
        {
            camera.transform.position = aspect16to9.position;
        }
        else if(camera.aspect >= 1.48)
        {
            camera.transform.position = aspect3to2.position;
        }
        else
        {
            camera.transform.position = aspect4to3.position;
        }*/
    }

	#endregion

	#region General functions

	private IEnumerator LoadGame(bool silent)
	{
        LoadingScreenParamContainer.Instance.SetupLoadScreen(SceneManager.GetActiveScene().name, levelName);

        loadLevelAsyncOperation = SceneManager.LoadSceneAsync("LoadingScreen", LoadSceneMode.Additive);
		//loadLevelAsyncOperation.allowSceneActivation = false;

		while (loadLevelAsyncOperation.progress < 0.9f) 
		{
			yield return null;
		}

		//loadLevelAsyncOperation.allowSceneActivation = false;
		finishedLoading = true;

		if(!silent)
		{
			LoadIsFinished();
		}
	}
		
	private void LoadIsFinished()
	{
        //SceneManager.UnloadScene("Startscreen");

        /*
		iTween.MoveTo(camera.gameObject, iTween.Hash(
			"position", positionOfGameCamera.position,
			"time", 1.0f,
			"delay", 0,
			"easetype", "easeInOutQuad",
			"onComplete", "StartGame",
			"onCompleteTarget", gameObject
		));
        */      
	}

	private void StartGame()
	{
		loadLevelAsyncOperation.allowSceneActivation = true;
	}

	#endregion

	#region reward garden

	public bool IsRewardGardenSet()
	{
		return PlayerPrefs.GetInt("RewardGardenStatus", 0) == 1 ? true : false;
	}

	public bool IsSeedInThePocket()
	{
		return saveGameController.GetCountUnplacedSeeds() > 0 ? true : false;
	}

	public int GetCountUnplacedSeeds()
	{
		return saveGameController.GetCountUnplacedSeeds();
	}

	public int GetCountPlacedSeeds()
	{
		return saveGameController.GetCountPlacedSeeds();
	}

	public int GetCountCollectedSeeds()
	{
		return saveGameController.GetCountCollectedSeeds();
	}

	public PlayerSaveGame.SeedPlace[] LoadRewardGarden()
	{
		return saveGameController.GetSeedPlaces();
	}

	public void AddSeedToCollection()
	{
		++saveGameController.current.collectedSeeds;
		SaveData();
	}

	public void RemoveSeedFromPocket()
	{
		++saveGameController.current.usedSeeds;
		SaveData();
	}

    public void IncreaseStartupCount()
    {
        saveGameController.IncreaseStartupCount();
        SaveData();
    }

	public void AddNewPlant(int seedType, Vector3 position, SpriteRenderer spriteRenderer)
	{
		saveGameController.AddNewPlant(seedType, position, spriteRenderer);
		SaveData();
	}

	public void SaveData()
	{
		try 
		{
			saveGameController.SaveData();

			if(OnSeedCountChange != null)
				OnSeedCountChange(GetCountUnplacedSeeds());
		} 
		catch (System.Exception ex) 
		{

		}
	}

    public void ShowMoreApps()
    {
        //webPromoSDK.ShowMoreApps();
        AudioManager.Instance.FadeOut(AudioManager.Instance.SFXChannels[0], 0.33f, 0.0f, FadeOutBGM);
    }

    private void FadeOutBGM()
    {
        AudioManager.Instance.FadeOut(AudioManager.Instance.BackgroundChannel, 1.0f);
    }

    public void QuitMoreApps()
    {
        Debug.Log("QuitMoreApps");
        uiController.HideMoreAppsPanel();
        AudioManager.Instance.FadeIn(AudioManager.Instance.SFXChannels[0], 0.33f, 0.0f, 1.0f, FadeInBGM);
    }

    private void FadeInBGM()
    {
        AudioManager.Instance.FadeIn(AudioManager.Instance.BackgroundChannel, 1.0f);
    }

	public void LoadData()
	{
		try 
		{
			saveGameController.LoadData();

            // At Frist Start
            if(saveGameController.GetStartUpCount() == 0)
            {
                AtFristStart();
            }

            // Language
            InitLanguage();

            // Run Callbacks
			if(OnSeedCountChange != null)
			{
				OnSeedCountChange(GetCountUnplacedSeeds());
			}

			if(OnLoadSaveGameComplete != null)
			{
				OnLoadSaveGameComplete();
			}
		} 
		catch (System.Exception ex) 
		{
			
		}
	}

    public void AtFristStart()
    {
        saveGameController.EnableTutorial();
    }

	public void ClearData()
	{
		saveGameController.ClearData();

		if(OnSeedCountChange != null)
			OnSeedCountChange(GetCountUnplacedSeeds());

		if (OnClearGameComplete != null)
			OnClearGameComplete();
	}

	private void OnApplicationQuit()
	{
		SaveData();
	}

    public void ClearGarden()
    {
        saveGameController.current.usedSeeds = 0;
        saveGameController.current.seedPlaces = new PlayerSaveGame.SeedPlace[50];
        saveGameController.DisableSecret();
        saveGameController.EnableTutorial();

        saveGameController.SaveData();

        // Run Callbacks
        if(OnSeedCountChange != null)
        {
            OnSeedCountChange(GetCountUnplacedSeeds());
        }

        if(OnLoadSaveGameComplete != null)
        {
            OnLoadSaveGameComplete();
        }
    }

	#endregion

    #region Language

    public void InitLanguage()
    {
        if(saveGameController.GetStartUpCount() > 0)
        {
            LocalisationController.Instance.ChangeLanguage(saveGameController.GetLanguage());
        }
        else
        {
            // -- First Start --
            LocalisationController.Instance.ChangeLanguage(Application.systemLanguage);
            Debug.Log("APPLICATION SYSTEM LANGUAGE: " + Application.systemLanguage);
            saveGameController.SetLanguage(LocalisationController.Instance.CurrentLanguageIndex);
            SaveData();
        } 
    }

    public void ChangeLanguage(int languageIndex)
    {
        LocalisationController.Instance.ChangeLanguage(languageIndex);
        saveGameController.SetLanguage(LocalisationController.Instance.CurrentLanguageIndex);
        SaveData();
    }

    #endregion

    public void QuitApp()
    {
        Application.Quit();
    }
}