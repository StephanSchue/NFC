using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Events;

public delegate void DiceRoleComplete(DiceFieldType id, FruitColor color);
public delegate void DiceRole();
public delegate void FruitIsAddedToTheBasket(FruitColor color);

[System.Serializable]
public class GameController : MonoBehaviour
{
    public EnvironmentController environmentController;

    public RavenController Raven;
	public ColorWheelController DiceController;
	public BasketController BasketController;
	public InputManager InputManager;

	public FruitColor[] imageColorList;

	public UIController uiController;

	public SpriteRenderer greyBackground;
	public FruitColor[] possibleFruitColors;

	public Transform[] TreePositions;

	public string possibleTreesList;
	public GameObject[] possibleTreePrefabs;

    public GameObject RewardAnimation;

	// Debug text labels
	public Text DebugText;
	public Text RavenPosDebugText;

	private GameObject[] placedTrees = null;

	// The colour we are looking for, asigned by the dice after every roll
	public FruitColor colourID = FruitColor.None;

	// RavenMinigame vars
	public RavenGameController[] ravenMinigames;
    public int[] positionOfTheMinigame;
    public int[] positionOfTheBubble;
    public int bubblePossibility = 2;

    public AudioClip rewardCheerSFX;

    public CameraController cameraController;

	/*
	public float ravenMinigameNotHappens;
	public float[] ravenMinigameLiklyhood; 
	*/

	public int ravenPositionsCount = 5;
	private int ravenPosition = 5;
	private int currentRavenGameIndex = 0;
	private RavenGameController currentRavenGame;
	public Transform[] ravenPositions;

	private FruitToColor fruitToColorController;

	public bool randomSpawnPositions = true;

	public bool GameIsFinished { get; private set; }

	// Fruit counter
	public int fruitCount;

	// Index is = FruitColor-1
	public int[] specificFruitCount = new int[4];

	public string[] treeFolders = new string[] { "Assets/Alpha/RandomizerExport" };

    // SaveGame
    private PlayerSaveGameController saveGameController;

    // Hints
    public bool fruitHint = false;
    public bool fruitHint1Active = false;
    public bool fruitHint2Active = false;
    public float timeBeforeFruitHighlightHint = 5.0f;
    private float fruitHighlightHintTime = 0.0f;
    public float timeBeforeFruitHighlightHint2 = 10.0f;
    private float fruitHighlightHint2Time = 0.0f;

    public float ravenStartupDelay = 2.0f;

    public TreeController currentHighlightedTree = null;
    public GameObject HintHand;

    public bool gameIsReady { get; private set; }

    // Events
    public DiceRoleComplete OnDiceRoleComplete = null;
    public DiceRole OnDiceRole = null;
    public FruitIsAddedToTheBasket OnFruitIsAddedToTheBasket = null;

    public TutorialController tutorialController;
    public bool tutorial = false;
    public bool forceTutorial = false;

    private string[] tutorialFruitList = new string[] { "RedCherries", "GreenApple", "YellowPears", "VioletPlum" };
    private FruitColor[] tutorialColorList = new FruitColor[] { FruitColor.Red, FruitColor.Green, FruitColor.Yellow, FruitColor.Violett };

    private DiceFieldType[] tutorialSpinFields = new DiceFieldType[] 
    { 
            DiceFieldType.One, DiceFieldType.Two, DiceFieldType.Three, DiceFieldType.Four, DiceFieldType.Raven, DiceFieldType.Two, DiceFieldType.Raven,  
            DiceFieldType.Four, DiceFieldType.Three, DiceFieldType.Two, DiceFieldType.One,
            DiceFieldType.Three, DiceFieldType.Four, DiceFieldType.Raven, DiceFieldType.One
    };
    private int tutorialSpinIndex = 0;

    private bool finishedLoading = false;
    public bool startLoading = false;
    private AsyncOperation loadLevelAsyncOperation;

    public RewardAnimationController rewardAnimationController;
    public bool showPointerAtColorWheelTutorial = false;

    private bool rewardSeedAnimationIsPlaying = false;
    private bool rewardRewardStartUpAnimation = false;

    public bool isGoingBackToMenu = false;

    private string[] deviceWhiteList = new string[] 
    { 
        "OS 10", "OS 11", "OS 12",
        "API-23", "API-24", "API-25", "API-26",
    };

    public UnityEvent onApplicationReady;

	#region Initalize

	/// <summary>
	/// Use this for initialization
	/// </summary>
	private void Start()
	{ 
		fruitToColorController = GetComponent<FruitToColor>();
		SetupScene();
	}

	/// <summary>
	/// Setups the scene.
	/// </summary>
	private void SetupScene()
	{
		GameIsFinished = false;

        // --- 0. Loading SaveFiles ---
        saveGameController = new PlayerSaveGameController();
        saveGameController.LoadData();

        LocalisationController.Instance.ChangeLanguage(saveGameController.current.language);
    
        tutorial = forceTutorial ? true : saveGameController.IsTutorialEnabled();

		// --- 1. Recreate the trees ---
		SpawnTrees();

		// --- 2. Reset position of raven ---
		ravenPosition = ravenPositionsCount;
		
		InputManager.Initalize();

		// -- Change the debug on screen text --
		DebugText.text = "";
		RavenPosDebugText.text = "Raven Position: " + ravenPosition;

		// --- 3. Count all the fruit in the scene ---
		CountFruits();
		DiceController.SetupWheelColors(imageColorList);
        DiceController.AllowToRoll(false);

        // --- 4. StartRavenAnimation & Enable Dice ---
        Raven.SetParticleSortingLayer(ravenPositionsCount - ravenPosition);

        if(tutorial)
        {
            Raven.SetTutorialMode(true);
            ResetRaven();
            MakeRavenInvisible();
            tutorialController.StartTutorial(TutorialEnded);
        }
        else
        {
            StartCoroutine(DoFunctionWithDelay(DoRavenStartUp, ravenStartupDelay));
            onApplicationReady.Invoke();
        }

        InputManager.BlockInput();
        rewardSeedAnimationIsPlaying = false;
	}

    private void DoRavenStartUp()
    {
        MakeRavenInvisible();
        ResetRaven();
        Raven.StartUp(ActivateDice);

        StartCoroutine(DoFunctionWithDelay(MakeRavenVisible, 0.1f));
    }

    public void ShowPointerAtColorWheel()
    {
        //Debug.Log("Lets Go");

        // --- Step 6.1 ---
        if(!showPointerAtColorWheelTutorial)
            return;

        //Debug.Log("Zeig mal!");
        EnablePointerFingerSwipeAnimation();

        showPointerAtColorWheelTutorial = false;
    }

    public void DisablePointerAtColorWheelHint()
    {
        showPointerAtColorWheelTutorial = false;
    }

    private IEnumerator DoFunctionWithDelay(FruitHighlightCallback method, float delay)
    {
        yield return new WaitForSeconds(delay);
        method();
    }

    private IEnumerator DoFunctionWithDelay(FruitHighlightAnimatorCallback method, Animator animator, float delay)
    {
        yield return new WaitForSeconds(delay);
        method(animator);
    }

    public void TutorialEnded()
    {
        Raven.SetTutorialMode(false);
        Debug.Log("Tutorial beendet!");
    }

	private void ResetRaven()
	{
		Raven.transform.position = ravenPositions[0].position;
		Raven.transform.rotation = ravenPositions[0].rotation;
        Raven.gameObject.SetActive(true);
	}

    public void SetRavenPosition(int positionIndex)
    {
        if(positionIndex > ravenPositionsCount)
            return;

        ravenPosition = ravenPositionsCount - positionIndex;
    }

    public void InstantMovePosition()
    {
        Raven.transform.position = ravenPositions[ravenPositionsCount - ravenPosition].position;
        Raven.transform.rotation = ravenPositions[ravenPositionsCount - ravenPosition].rotation;


        if(ravenPosition == 0)
        {
            // On the last Step
            Raven.SetAtTheBasket(true);
            Raven.IdleAtBasket(null);
        }
        else
        {
            Raven.SimpleIdle(null);
        }
    }

    public void InstantMovePositionWithOutAnimation()
    {
        Raven.transform.position = ravenPositions[ravenPositionsCount - ravenPosition].position;
        Raven.transform.rotation = ravenPositions[ravenPositionsCount - ravenPosition].rotation;
    }

    public void MakeRavenVisible()
    {
        Raven.DoVisible();
        gameIsReady = true;
    }

    public void MakeRavenInvisible()
    {
        Raven.DoInvisible();
    }

	public void ActivateDice()
    {
        uiController.HideFinishScreen();
        DiceController.AllowToRoll(true);
        DiceController.Enable(true);
        InputManager.UnblockInput();
		InputManager.UnblockDiceRole();
        uiController.EnableDiceButton(true); // Debug

        Raven.Idle(null);

        if(IsDeviceOnWhiteList())
        {
            environmentController.StartMole();
            environmentController.StartBee();
            environmentController.StartPollen();
        }

        if(!tutorial)
        {
            showPointerAtColorWheelTutorial = true;
            //StartCoroutine(DoFunctionWithDelay(ShowPointerAtColorWheel, 5.0f));
        }
    }

    private bool IsDeviceOnWhiteList()
    {
        if(SystemInfo.deviceType == DeviceType.Desktop)
            return true;

        for(int i = 0; i < deviceWhiteList.Length; i++)
        {
            if(SystemInfo.operatingSystem.IndexOf(deviceWhiteList[i]) >= 0)
            {
                Debug.Log("White list device");
                return true;
            }
        }

        Debug.Log("Black list device " + SystemInfo.operatingSystem);
        return false;
    }

	/// <summary>
	/// Reload the hole scene
	/// </summary>
	public void ResetPage()
	{
        iTween.Stop();
		Scene scene = SceneManager.GetActiveScene();
        SceneManager.UnloadScene(scene.name);
		SceneManager.LoadScene(scene.name);
	}

    /// <summary>
    /// Load the MainMenu
    /// </summary>
    public void BackToMenu()
    {
        if(isGoingBackToMenu)
            return;

        StartCoroutine(LoadGame("StartScreen", false));
        isGoingBackToMenu = true;
    }

    /// <summary>
    /// Load the MainMenu
    /// </summary>
    public void BackToMenu(float delay)
    { 
        if(isGoingBackToMenu)
            return;
        
        StartCoroutine(LoadGame("StartScreen", false, delay));
        isGoingBackToMenu = true;
    }

    public void ShowRewardStartUpAnimation(int seedCount, int avaibleSeeds)
    {
        if(rewardRewardStartUpAnimation)
            return;

        RewardAnimation.SetActive(true);
        rewardAnimationController.PrepairSeeds(seedCount, avaibleSeeds);
        rewardAnimationController.StartRewardAnimation(ActivateRewardStartUpAnimation);
    }

    public void ActivateRewardStartUpAnimation()
    {
        rewardRewardStartUpAnimation = true;

        StartCoroutine(DoFunctionWithDelay(ShowRewardSeedIntoBackAnimation, 5.0f));
    }

    public void ShowRewardSeedIntoBackAnimation()
    {
        if(!rewardRewardStartUpAnimation || rewardSeedAnimationIsPlaying)
            return;

        rewardAnimationController.StartSeedIntoBackAnimation();
        rewardSeedAnimationIsPlaying = true;
    }

    private IEnumerator LoadGame(string levelName, bool silent, float delay = 0.0f)
    {
        LoadingScreenParamContainer.Instance.SetupLoadScreen(SceneManager.GetActiveScene().name, levelName);

        if(delay > 0.0f)
        {
            yield return new WaitForSeconds(delay);
        }

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
         
    }

	/// <summary>
	/// Only for public usage
	/// </summary>
	public void SpawnTrees()
	{
		imageColorList = new FruitColor[4];

        if(tutorial)
        {
            imageColorList = tutorialColorList;
        }
        else
        {
            imageColorList = Shuffle<FruitColor>(possibleFruitColors);
        }

		/*
		for(int i = 0; i < possibleTreePrefabs.Length; i++) 
		{
			if(possibleTreePrefabs[i].GetComponent<TreeController>().IsColorPossible(
		}
		*/

        SpawnRandomTrees(imageColorList);

        float delay = 1.5f;

        for (int i = 0; i < TreePositions.Length; i++)
        {
            StartCoroutine(DoFunctionWithDelay(EnableAnimator, TreePositions[i].GetComponent<Animator>(), delay));
            delay += 0.25f;
        }
	}

    private void EnableAnimator(Animator animatorObject)
    {
        if(animatorObject == null)
            return;

        animatorObject.enabled = true;
    }

	/// <summary>
	/// Spawns the random trees.
	/// </summary>
	private void SpawnRandomTrees(params FruitColor[] possibleColors)
	{
		// Clear old Trees
		ClearTrees();

		if (TreePositions.Length <= 0)
			return;

		// Instanziate
		GameObject placedTree = null;
		TreeController placedTreeController = null;
		TreeController possibleTreePrefabsController = null;
		int fruitPresetIterator = 0;

		// Shuffle positions & prefabs
		Transform[] treePositionsShuffled = randomSpawnPositions ? Shuffle<Transform>(TreePositions) : TreePositions;
		placedTrees = new GameObject[treePositionsShuffled.Length];
		
        if (!tutorial)
        {
            possibleTreePrefabs = Shuffle<GameObject>(possibleTreePrefabs);
        }

        int tutorialFruitListIndex = 0;

		// Place prefabs
		for (int i = 0; i < treePositionsShuffled.Length; i++) 
		{
			if (i < possibleTreePrefabs.Length) 
			{
				if(possibleTreePrefabs[i] == null) 
				{
					Debug.Log("Please go to GameController and Press 'Load Tree Prefabs'");
				} 
				else 
				{
					possibleTreePrefabsController = possibleTreePrefabs[i].GetComponent<TreeController>();

					// --- Only one Tree for each color ---
					if (possibleTreePrefabsController.IsColorPossible(possibleColors[fruitPresetIterator])) 
					{
						// - Place prefab -
						placedTree = Instantiate<GameObject>(possibleTreePrefabs[i]);
						placedTree.transform.position = treePositionsShuffled[i].position;
						placedTree.transform.rotation = treePositionsShuffled[i].rotation;
						placedTree.transform.parent = treePositionsShuffled[i];
						placedTree.transform.localScale = Vector3.one;
						placedTrees[i] = placedTree;

						placedTreeController = placedTree.GetComponent<TreeController>();

						// - Place fruits -
						if(placedTreeController) 
                        {
                            if (tutorial)
                            {
                                // Add fruits set by tutorial
                                placedTreeController.CreateTreeFruits(tutorialFruitList[tutorialFruitListIndex++]);
                            }
                            else
                            {
                                // Add possible fruits
                                placedTreeController.CreateTreeFruits(possibleColors[fruitPresetIterator]);
                            }

							imageColorList[fruitPresetIterator] = possibleColors[fruitPresetIterator];
						}

						++fruitPresetIterator;
					}

					if (fruitPresetIterator >= possibleColors.Length) 
					{
						return;
					}
				}
			}
		}
	}

	/// <summary>
	/// Count the fruits on the current playground.
	/// </summary>
	private void CountFruits()
	{
		// TODO: importend Number
		specificFruitCount = new int[Enum.GetNames(typeof(FruitColor)).Length-1];
		fruitCount = 0;

		GameObject[] _fruits = GameObject.FindGameObjectsWithTag("Fruit");
		FruitController fruitController = null;

		if (_fruits != null && _fruits.Length > 0) 
		{
			// -- Set the total count --
			fruitCount = _fruits.Length;

			// -- Read the count specific --
			for(int i = 0; i < _fruits.Length; i++) 
			{
				fruitController = _fruits[i].GetComponent<FruitController>();

                if (fruitController == null)
                {
                    Debug.Log(_fruits[i].name);
                }

				if(GetSpecificFruitIndex(fruitController.iD) > -1)
				{
					specificFruitCount[GetSpecificFruitIndex(fruitController.iD)]++;
				}
			}
		}

		fruitController = null;
		_fruits = null;
	}

	#endregion

	#region GameLoop

	/// <summary>
	/// Update this instance.
	/// </summary>
	private void Update()
	{
        if(fruitHint)
        {
            if(!fruitHint1Active)
            {
                if (Time.time > fruitHighlightHintTime)
                {
                    fruitHint1Active = true;
                    SetFruitHintTwoTime();
                    ShowFruitHintOne();
                }
            }
            else if(!fruitHint2Active)
            {
                if (Time.time > fruitHighlightHint2Time)
                {
                    fruitHint2Active = true;
                    ShowFruitHintTwo();
                }
            }
        }

		// --- RavenGame ---
		if (!currentRavenGame)
			return;
		
		if (!currentRavenGame.miniGameIsRunning)
			return;
		
		if(currentRavenGame.timeWhenMinigameEnd > Time.time) 
        {
			//DebugText.text = "Tap the screen before the timer runs out, you have: \n" + (minigameTimer - Time.time).ToString("0") + " sec";

			// - Check clean up the screen -
			if (Input.GetMouseButtonUp (0)) {
				currentRavenGame.ClearAction ();
			} else if (Input.GetMouseButton (0)) {
				currentRavenGame.DoAction();
			}

			// - Check mask already cleaned up -
			if (currentRavenGame.WinConditionsAreSovled()) 
			{
				currentRavenGame.WinTheGame(RavenMinigameEnd);
			}
		} else {
			// - Lose Game -
			currentRavenGame.LooseTheGame(RavenMinigameEnd);
		}
	}

	#endregion

	#region Gameplay

	/// <summary>
	/// Roles the dice.
	/// </summary>
    public void RoleDice(bool directionRight=true)
	{
		DiceController.StopBlinking();

        if(DiceController.IsRollAllowed())
        {
            if (tutorial == true && tutorialSpinIndex < tutorialSpinFields.Length)
            {
                DiceController.Roll(tutorialSpinFields[tutorialSpinIndex++], directionRight);
            }
            else
            {
                DiceController.Roll(directionRight);
            }

            InputManager.BlockInput();

			if(DiceController.IsSwipeHindEnabled())
			{
				DisablePointerFingerSwipeAnimation();
			}

            if(OnDiceRole != null)
                OnDiceRole();
        }
	}

    public void StartBublbeGame()
    {
        currentRavenGameIndex = 0;
        ravenPosition = 2;

        Raven.transform.position = ravenPositions[ravenPositionsCount - ravenPosition].position;
        Raven.transform.rotation = ravenPositions[ravenPositionsCount - ravenPosition].rotation;

        StartMiniGame();
    }

    public void StartFishingGame()
    {
        currentRavenGameIndex = 1;
        ravenPosition = 2;

        Raven.transform.position = ravenPositions[ravenPositionsCount - ravenPosition].position;
        Raven.transform.rotation = ravenPositions[ravenPositionsCount - ravenPosition].rotation;

        StartMiniGame();
    }

    public void StartHelicopterGame()
    {
        currentRavenGameIndex = 2;
        ravenPosition = 2;

        Raven.transform.position = ravenPositions[ravenPositionsCount - ravenPosition].position;
        Raven.transform.rotation = ravenPositions[ravenPositionsCount - ravenPosition].rotation;

        StartMiniGame();
    }

	public void TriggerRavenWinGame()
	{
		ravenPosition = 0;

		Raven.transform.position = ravenPositions[ravenPositionsCount - ravenPosition].position;
		Raven.transform.rotation = ravenPositions[ravenPositionsCount - ravenPosition].rotation;

		RavenWinGame();
	}

	/// <summary>
	/// Function after dice role to set the result
	/// </summary>
	/// <param name="id">Identifier.</param>
	public void SetDiceResult(DiceFieldType id)
    {
		colourID = GetFruitColorToDiceNumber(id);
        DiceController.HightLightSelected(id);
        ActivateFruitHint();

        if (OnDiceRoleComplete != null)
            OnDiceRoleComplete(id, GetFruitColorToDiceNumber(id));

		if (id == DiceFieldType.Raven) 
		{
			// Shuffle Dice Minigsame
			DiceController.AllowToRoll(false);
			DiceController.Enable(false);
			uiController.diceButton.enabled = false;

			//Handheld.Vibrate();

			int index = ravenPositionsCount - ravenPosition; //CalculateRavenMiniGameLiklyhood();
            bool miniGame = false;
            bool miniGameBubble = false;

            for (int i = 0; i < positionOfTheMinigame.Length; i++)
            {
                if(positionOfTheMinigame[i] == index)
                {
                    miniGame = true;
                }
            }

            if(!tutorial && UnityEngine.Random.Range(0, 100) <= bubblePossibility)
            {
                for (int i = 0; i < positionOfTheBubble.Length; i++)
                {
                    if(positionOfTheBubble[i] == index)
                    {
                        miniGameBubble = true;
                    }
                }
            }

            if((miniGame || miniGameBubble) && ravenMinigames != null && ravenMinigames.Length > 0) //index >= 0 && index < ravenMinigames.Length && ravenMinigames[index] != null) 
            {
				// Start Animation, after completing run callback on method "StartMiniGame"
				--ravenPosition;

                if(miniGameBubble)
                {
                    // Bubble Game
                    currentRavenGameIndex = 0;
                }
                else
                {
                    // Random Game
                    currentRavenGameIndex = tutorial ? 0 : UnityEngine.Random.Range(0, ravenMinigames.Length);
                }

				RavenHide(StartMiniGame);  
			} 
			else 
			{
                if(ravenPosition > 0)
                {
                    Debug.Log("No Minigame implementent for Position " + (index));
                    currentRavenGameIndex = -1;
                    MoveRaven();
                    --ravenPosition;
                }
                else
                {
                    RavenWinGame();
                }
			}
		} 
		else 
		{
			DiceController.AllowToRoll(false);
			InputManager.UnblockInput();
			InputManager.BlockDiceRole();
            InputManager.blockGrabing = false;
            uiController.diceButton.enabled = false;
		}
	}
        
	public void StartMiniGame()
	{
		RavenMinigameStart(currentRavenGameIndex);
	}

	/// <summary>
	/// Ravens the minigame start.
	/// </summary>
	/// <returns>The minigame start.</returns>
	/// <param name="waitTime">Wait time.</param>
	public void RavenMinigameStart(int index)
	{
		Raven.transform.position = ravenPositions[ravenPositionsCount - ravenPosition].position;
		Raven.transform.rotation = ravenPositions[ravenPositionsCount - ravenPosition].rotation;

		// Disable the dice roll
		InputManager.enabled = false;
		DiceController.AllowToRoll(false);
        cameraController.enabled = false;

		// Randomly select one of the minigames and create a local version of the minigame parameters
		currentRavenGame = ravenMinigames[index];

		// Start MiniGame
        currentRavenGame.StartMiniGame(tutorial);
	}

	/// <summary>
	/// Ravens minigame ended.
	/// </summary>
	private void RavenMinigameEnd(bool status)
	{
		// Re-enable the dice roll
		InputManager.enabled = true;
		DiceController.AllowToRoll(true);
		DiceController.Enable (true);
		InputManager.UnblockInput();
		InputManager.UnblockDiceRole();
        cameraController.enabled = true;

		Raven.GetComponentInChildren<MeshRenderer>().enabled = true;

		// --- Check if you won or lost the minigame ---
		if (status) 
		{
			// -- WIN --
			// Change the debug on screen text
			DebugText.text = "Great Job!";

			// --- Raven Movement Stuff ---
			Raven.transform.position = ravenPositions[ravenPositionsCount - ravenPosition].position;
			Raven.transform.rotation = ravenPositions[ravenPositionsCount - ravenPosition].rotation;

            Raven.Pout(RandomIdle);
            //ravenPosition -= 1;

			DiceController.AllowToRoll(true);
			DiceController.Enable(true);
			uiController.diceButton.enabled = true;

            DiceController.StartBlinking();
			// -------
		} 
		else 
		{
			// -- LOOSE --
			// Change the debug on screen text
			DebugText.text = "Too late";
            
			// --- Raven Movement Stuff ---
            //RavenMovedComplete();
			// -------

            Raven.Pout(RandomIdle);

			// Count down the position of the Raven
			//ravenPosition -= 1;
		}

		Debug.Log("Raven Position: " + ravenPosition);
	}

    public void RandomIdle()
    {
        Raven.SetIdleStatus(true);
        Raven.Idle(RandomIdle);
        Debug.Log("Random Idle");
    }

	/// <summary>
	/// Moves the raven.
	/// </summary>
	public void MoveRaven()
	{
        RavenHide(RavenHidded);
	}

	public void RavenHide(UnityAction callback=null)
	{
        Raven.Dive(ravenPositionsCount - ravenPosition, callback);
	}

	/// <summary>
	/// Raven complete movement.
	/// </summary>
    public IEnumerator RavenMovedComplete()
	{
		Raven.transform.position = ravenPositions[ravenPositionsCount - ravenPosition].position;
		Raven.transform.rotation = ravenPositions[ravenPositionsCount - ravenPosition].rotation;

        yield return new WaitForSeconds(1.0f);

        Raven.Appear(null);

        DiceController.StartBlinking();

		// Change the debug on screen text
		RavenPosDebugText.text = "Raven Position: " + ravenPosition;

		// Player lose
		if (ravenPosition < 0) 
		{
			FinishGame(false);
		} 
		else 
		{
            InputManager.UnblockInput();
			DiceController.AllowToRoll(true);
			DiceController.Enable(true);
			InputManager.UnblockDiceRole();
			uiController.diceButton.enabled = true;
		}

        if(ravenPosition == 0)
        {
            // On the last Step
            Raven.SetAtTheBasket(true);
            Raven.Appear(Raven.IdleAtBasket);
        }
	}

    public void RavenHidded()
    {
        StartCoroutine(RavenMovedComplete());
    }

	// The things below are just for the debug, to give text feedback to the user
	// TODO: replace with actual logic and animations
	public void AddedFruitToTheBasket(FruitColor id)
	{
		// -- Count down the fruit amount --
		fruitCount -= 1;
		int fruitCountIterator = GetSpecificFruitIndex(id);
		
		// --- Decrease added fruit ---
		if (fruitCountIterator > -1) 
		{
			specificFruitCount[fruitCountIterator]--;

			if(specificFruitCount[fruitCountIterator] == 0) 
			{
                if(fruitCount > 0)
				    DiceController.StartBlinking(0.0f);
			}
				
		}

        if(OnFruitIsAddedToTheBasket != null)
        {
            OnFruitIsAddedToTheBasket(id);
			InputManager.UnblockInput();
        }
            
		print (id.ToString () + " in the basket");
		print ("Fruits remaining: " + fruitCount);

		// -- Finish the Game --
        if(fruitCount == 0)
        {
            // --- Debug Stuff ---
            DisablePointerAtColorWheelHint();
            DisablePointerFingerSwipeAnimation();

            // Disable Dice
            DiceController.HideColorWheel();
            DiceController.Enable(false);
            DiceController.AllowToRoll(false);
            uiController.EnableDiceButton(false); // Debug

            InputManager.BlockInput();
            InputManager.BlockDiceRole();

            // Win Game
            Raven.transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
            Raven.RavenLoss(WinTheGame);
            uiController.backButton.gameObject.SetActive(false);
        }
        else
        {
            // --- Normal GAME ---

            // Allow Only One fruit
            colourID = FruitColor.None;
            DiceController.StartBlinking(0.0f);
            DiceController.AllowToRoll(true);
            DiceController.Enable(true);
            uiController.diceButton.enabled = true;
            InputManager.UnblockDiceRole();
            InputManager.blockGrabing = true;
            DiceController.UnHightLightSelected();

            if(!tutorial)
            {
                //showPointerAtColorWheelTutorial = true;
                //StartCoroutine(DoFunctionWithDelay(ShowPointerAtColorWheel, 5.0f));
            }
        }
	}

    public void RemoveFruitFromTheBasket(FruitColor id)
    {
        // -- Count down the fruit amount --
        fruitCount += 1;
        int fruitCountIterator = GetSpecificFruitIndex(id);

        // --- Decrease added fruit ---
        if (fruitCountIterator > -1) 
        {
            specificFruitCount[fruitCountIterator]++;
        }

        print (id.ToString () + " in the basket");
        print ("Fruits remaining: " + fruitCount);
    }

	/// <summary>
	/// Show Finish Screen and time to reset all stuff
	/// </summary>
	public void FinishGame(bool win)
	{
		GameIsFinished = true;
        Raven.gameObject.SetActive(false);
        uiController.backButton.gameObject.SetActive(false);

		if (greyBackground) {
			greyBackground.enabled = true;
		}

		// Disable Dice
        DisablePointerAtColorWheelHint();
        DisablePointerFingerSwipeAnimation();

        DiceController.HideColorWheel();
        DiceController.Enable(false);
        DiceController.AllowToRoll(false);
		uiController.EnableDiceButton(false); // Debug

        InputManager.BlockInput();
        InputManager.BlockDiceRole();

        saveGameController.IncreaseGamesPlayed();

		// Show Win/Lose
		if(win) 
        {
            uiController.ShowWinCutscene();
            
            if(rewardCheerSFX != null)
            {
                AudioManager.Instance.SetSFXChannel(rewardCheerSFX);
            }

            int seedCount = UnityEngine.Random.Range(1, 4);
            ShowRewardStartUpAnimation(seedCount, saveGameController.current.collectedSeeds - saveGameController.current.usedSeeds);

            saveGameController.current.collectedSeeds += (byte)seedCount;
            saveGameController.DisableTutorial();
            saveGameController.SaveData();
		} 
        else 
        {
			//uiController.ShowLoseCutscene();
			BackToMenu();
            saveGameController.SaveData();
		}
	}

    public void RavenWinGame()
    {
        DiceController.HideColorWheel();
        DiceController.AllowToRoll(false);
        DiceController.Enable(false);
        uiController.diceButton.enabled = false;
        InputManager.BlockInput();

        uiController.backButton.gameObject.SetActive(false);
        Raven.BasketAndRun(LooseTheGame);
    }


    public void LooseTheGame()
    {
        FinishGame(false);
    }

    public void WinTheGame()
    {
        FinishGame(true);
    }
                    
	/// <summary>
	/// Gets the specific fruit count.
	/// </summary>
	/// <returns>The specific fruit count.</returns>
	/// <param name="id">Identifier.</param>
	public int GetSpecificFruitCount(FruitColor id)
	{
		int count = 0;

		if(GetSpecificFruitIndex(id) > -1) 
		{
			count = specificFruitCount[GetSpecificFruitIndex(id)];
		}

		return count;
	}

	/// <summary>
	/// Clears the trees on the playground.
	/// </summary>
	public void ClearTrees()
	{
		if (placedTrees != null && placedTrees.Length > 0) {
			for (int i = 0; i < placedTrees.Length; i++) {
				if (placedTrees [i] != null) {
					DestroyImmediate (placedTrees [i]);
				}
			}
		}
	}

	#endregion

	#region Utilities

	/// <summary>
	/// Resets the debug text.
	/// </summary>
	private void ResetDebugText()
	{
		// Change the debug on screen text
		DebugText.text = "";
	}

	/// <summary>
	/// Shuffle the array.
	/// </summary>
	/// <typeparam name="T">Array element type.</typeparam>
	/// <param name="array">Array to shuffle.</param>
	public T[] Shuffle<T> (T[] array)
	{
		System.Random random = new System.Random ();
		for (int i = array.Length; i > 1; i--) {
			// Pick random element to swap.
			int j = random.Next (i); // 0 <= j <= i-1
			// Swap.
			T tmp = array [j];
			array [j] = array [i - 1];
			array [i - 1] = tmp;
		}
		return array;
	}

	public FruitColor GetFruitColorToDiceNumber(DiceFieldType diceNumber)
	{
		FruitColor color = FruitColor.None;

		if((int)diceNumber > 0) 
		{
			color = imageColorList[GetSpecificFruitIndex(diceNumber)];
		}

		return color;
	}

    public DiceFieldType GetDiceNumberForFruitColor(FruitColor fruitColor)
    {
        DiceFieldType diceNumber = DiceFieldType.None;

        if((int)fruitColor > 0) 
        {
            for (int i = 0; i < imageColorList.Length; i++)
            {
                if(fruitColor == imageColorList[i])
                {
                    diceNumber = (DiceFieldType)(i+1);
                }
            }
        }

        return diceNumber;
    }

	public int GetSpecificFruitIndex(FruitColor value)
	{
		return (int)(value-1);
	}

	public int GetSpecificFruitIndex(DiceFieldType value)
	{
		return (int)(value-1);
	}


    #endregion

    #region fruit hint functions

    public void ActivateFruitHint()
    {
        fruitHint = true;
        SetFruitHintOneTime();
        Debug.Log("ActivateFruitHint");
    }

    public void SetFruitHintOneTime()
    {
        fruitHighlightHintTime = Time.time + timeBeforeFruitHighlightHint;
    }

    public void SetFruitHintTwoTime()
    {
        fruitHighlightHint2Time = Time.time + timeBeforeFruitHighlightHint2;
    }

    public void DeactivateFruitHint()
    {
        fruitHint = fruitHint1Active = fruitHint2Active = false;
        Debug.Log("DeactivateFruitHint");

        if(currentHighlightedTree != null)
            currentHighlightedTree.HighlightFruits(false);
        
        HidePointFinger();
    }

    public void ShowFruitHintOne()
    {
        Debug.Log("ShowFruitHintOne");

        int treeIndex = GetTreeIndexOfColor(colourID);

        if(treeIndex > placedTrees.Length || treeIndex < 0)
            return;

        currentHighlightedTree = placedTrees[GetTreeIndexOfColor(colourID)].GetComponent<TreeController>();

        Debug.Log(currentHighlightedTree);
        if(currentHighlightedTree != null)
        {
            currentHighlightedTree.HighlightFruits(true);
        }
    }

    public void ShowFruitHintTwo()
    {
        Debug.Log("ShowFruitHintTwo");

        int treeIndex = GetTreeIndexOfColor(colourID);

        if(treeIndex > placedTrees.Length || treeIndex < 0)
            return;

        if(currentHighlightedTree != null)
        {
            currentHighlightedTree = placedTrees[treeIndex].GetComponent<TreeController>();
        }

        if(currentHighlightedTree != null)
        {
            Vector3 offset = new Vector3(3.0f, 3.0f, 0.0f);

            if (treeIndex == 1 || treeIndex == 2)
            {
                transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
                offset = new Vector3(-offset.x, offset.y, offset.z);
            }
            else
            {
                transform.localScale = Vector3.one;
            }

            ShowPointFinger(currentHighlightedTree.TreeTop.position, offset);
            StartMovingHandToBasket(HintHand, BasketController.gameObject);
        }
    }

    public void StartMovingHandToBasket(GameObject hintHand, GameObject basket)
    {
        Vector3 offset = (Vector3.up * 2.0f);
        
        iTween.MoveTo(hintHand, iTween.Hash(
            "path", new Vector3[] { hintHand.transform.position, basket.transform.position + offset },
            "time", 3.0f,
            "delay", 0.2f,
            "easetype", "linear",
            "loopType", iTween.LoopType.pingPong
        ));
    }

    public void StopMovingHandToBasket()
    {
        iTween.Stop(HintHand);
    }

    public void ShowPointFinger(Vector3 pointAt, Vector3 offset = default(Vector3))
    {
        HintHand.transform.position = pointAt + offset;

        Vector3 diff = pointAt - HintHand.transform.position;
        diff.Normalize();

        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        HintHand.transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);

        HintHand.SetActive(true);
    }

	public void EnablePointerFingerSwipeAnimation()
	{
		DiceController.EnableSwipeHint();
	}

	public void DisablePointerFingerSwipeAnimation()
	{
		DiceController.DisableSwipeHint();
	}

    public int GetTreeIndexOfColor(FruitColor color)
    {
        for (int i = 0; i < imageColorList.Length; i++)
        {
            if(color == imageColorList[i])
            {
                return i;
            }
        }

        return -1;
    }

    public void HidePointFinger()
    {
        StopMovingHandToBasket();
        HintHand.SetActive(false);
    }

	#endregion
}

public enum DiceFieldType
{
	None = 0,
	Raven = -1,
	One = 1,
	Two = 2,
	Three = 3,
	Four = 4,
}

public enum FruitColor
{
	None = 0,
	Red = 1,
	Yellow = 2,
	Blue = 3,
	Green = 4,
	Orange = 5,
	Violett = 6
}

public enum SwipeType
{
	None,
	Up,
	Down,
	Left,
	Right
}