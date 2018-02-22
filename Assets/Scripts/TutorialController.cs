using UnityEngine;
using System.Collections;

public delegate void FruitHighlightCallback();
public delegate void FruitHighlightAnimatorCallback(Animator animator);
public delegate void TutorialFinishedCallback();

public class TutorialController : MonoBehaviour, ILocalizedObject
{
    public static FruitHighlightCallback OnFruitHighlight;
    public static FruitHighlightCallback OnFruitUnHighlight;

    public RavenController ravenController;
    public GameController gameController;

    private AudioClip[] voiceOvers;
    private AudioClip[] voiceOversColor;
    private AudioClip[] voiceOversTheoAnnouncement;

    private int currentVoiceOverIndex = 0;

    private TutorialFinishedCallback tutorialFinishedCallback;
    private bool showPointerAtColorWheelTutorial = false;

    public UnityAction[] tutorialMethodSequence;
    private int tutorialMethodSequenceIndex = 0;

    private int currentFruitsToPickCount = 0;
    private int fruitsToPickCount = 4;

    private bool voiceOverOfColorDone = false;
    private bool playerCheeringDone = false;

	public float delayBetweenVoiceOver = 0.0f;

    public string default_iso;

    #region constuct & deconstruct

    protected void Start()
    {
        tutorialMethodSequence = new UnityAction[]
        {
            RavenFlyesIn,
            RavenLands,
            RavenIdles,
            GameStart,
            StartColorWheelTutorial
        };
    }

    protected void OnDestory()
    {
        LocalisationController.OnLanguageChanged -= OnLanguageChanged;
        gameController.OnFruitIsAddedToTheBasket -= OnAddToBasket;
        gameController.OnDiceRole -= DiceRoleClicked;
        gameController.OnDiceRoleComplete -= DiceRoleComplete;

        OnFruitHighlight = null;
        OnFruitUnHighlight = null;
    }

    #endregion

    #region Tutorial control functions

    public void StartTutorial(TutorialFinishedCallback callback)
    {
        OnLanguageChanged(LocalisationController.Instance.CurrentLanguage.isoCode);

        tutorialMethodSequenceIndex = 0;
        currentVoiceOverIndex = 0;
        currentFruitsToPickCount = 0;
        voiceOverOfColorDone = false;
        playerCheeringDone = false;

        LocalisationController.OnLanguageChanged += OnLanguageChanged;
        gameController.OnFruitIsAddedToTheBasket += OnAddToBasket;
        gameController.OnDiceRole += DiceRoleClicked;
        gameController.OnDiceRoleComplete += DiceRoleComplete;

        tutorialFinishedCallback = callback;
        Intro();
    }

    public void NextStep()
    {
        tutorialMethodSequence[(tutorialMethodSequenceIndex < tutorialMethodSequence.Length ? tutorialMethodSequenceIndex++ : tutorialMethodSequence.Length-1)]();
    }

    public void PreviousStep()
    {
        tutorialMethodSequence[(tutorialMethodSequenceIndex > 0 ? tutorialMethodSequenceIndex-- : 0)]();
    }

    #endregion

    #region Tutorial sequence functions

    private void Intro()
    {
        tutorialMethodSequenceIndex = 0;

        // --- Step 1 ---
		SetVoiceOver(0, RavenFlyesIn, delayBetweenVoiceOver);
        StartCoroutine(DoFunctionWithDelay(HightlightFruits, 0.1f));
    }

    private void RavenFlyesIn()
    {
        tutorialMethodSequenceIndex = 1;

        // --- Step 2 ---
		AudioManager.Instance.SetVoiceOver(voiceOversTheoAnnouncement[Random.Range(0, voiceOversTheoAnnouncement.Length)], RavenLands, delayBetweenVoiceOver);

        ravenController.LoopingAndLand(null);
        StartCoroutine(DoFunctionWithDelay(gameController.MakeRavenVisible, 1.0f));
    }

    public void RavenLands()
    {
        tutorialMethodSequenceIndex = 2;

        // --- Step 3 ---
		SetVoiceOver(1, RavenIdles, delayBetweenVoiceOver);
    }

    public void RavenIdles()
    {
        tutorialMethodSequenceIndex = 3;

        // --- Step 4 ---
		SetVoiceOver(2, GameStart, delayBetweenVoiceOver);
    }

    public void GameStart()
    {
        tutorialMethodSequenceIndex = 4;

        gameController.InputManager.UnblockInput();
        gameController.DiceController.Enable(true);
        gameController.DiceController.AllowToRoll(true);
        gameController.DiceController.HighlightWheel();
        gameController.InputManager.UnblockDiceRole();

        // --- Step 5 ---
		SetVoiceOver(3, StartColorWheelTutorial, delayBetweenVoiceOver);

        UnHightlightFruits();
    }

    public void StartColorWheelTutorial()
    {
        tutorialMethodSequenceIndex = 5;

        // --- Step 6 ---
        //gameController.DiceController.StartBlinking();
		
        showPointerAtColorWheelTutorial = true;
        StartCoroutine(DoFunctionWithDelay(ShowPointerAtColorWheel, 5.0f));
    }

    public void ShowPointerAtColorWheel()
    {
        tutorialMethodSequenceIndex = 6;

        // --- Step 6.1 ---
        if(!showPointerAtColorWheelTutorial)
            return;

		SetVoiceOver(4, StartColorWheelTutorial, delayBetweenVoiceOver);
        Vector3 fingerPosition = Camera.main.ScreenToWorldPoint(gameController.DiceController.transform.position + (Vector3.forward * 10.0f));
        
		gameController.EnablePointerFingerSwipeAnimation();
		//gameController.ShowPointFinger(fingerPosition + (Vector3.forward * 10.0f) + (Vector3.down * 1.0f)); 

        showPointerAtColorWheelTutorial = false;
    }

    public void DiceRoleClicked()
    {
        showPointerAtColorWheelTutorial = false;
        gameController.DiceController.UnHighlightWheel();
		gameController.DisablePointerFingerSwipeAnimation();
    }

    public void DiceRoleComplete(DiceFieldType diceNumber, FruitColor color)
    { 
        if (!voiceOverOfColorDone)
        {
            VoiceOverOfColor(color);
        }
    }

    public void OnAddToBasket(FruitColor color)
    {
        if(!playerCheeringDone && currentFruitsToPickCount >= fruitsToPickCount)
        {
            CheeringPlayer();
            playerCheeringDone = true;
        }
    }

    public void CheeringPlayer()
    {
		SetVoiceOver(5, CheeringPlayer2, delayBetweenVoiceOver);
    }

    public void CheeringPlayer2()
    {
		SetVoiceOver(6, TheoHintVoice, delayBetweenVoiceOver + 0.1f);
    }

    public void TheoHintVoice()
    {
		SetVoiceOver(7, null, delayBetweenVoiceOver + 0.2f);
    }

    private void TutorialIsFinished()
    {
        
    }

    #endregion

    #region Highlight functions

    private IEnumerator DoFunctionWithDelay(FruitHighlightCallback method, float delay)
    {
        yield return new WaitForSeconds(delay);
        method();
    }

    /// <summary>
    /// Event on Hightlight the fruits.
    /// </summary>
    public void HightlightFruits()
    {
        if(OnFruitHighlight != null)
            OnFruitHighlight();
    }

    /// <summary>
    /// Event on UnHightlight the fruits.
    /// </summary>
    public void UnHightlightFruits()
    {
        if(OnFruitUnHighlight != null)
            OnFruitUnHighlight();
    }

    #endregion

    #region voice over functions

    /// <summary>
    /// Gos to next voice over.
    /// </summary>
    public void GoToNextVoiceOver(UnityAction callback=null)
    {
        ++currentVoiceOverIndex;

        if(currentVoiceOverIndex < voiceOvers.Length)
        {
            SetVoiceOver(currentVoiceOverIndex, callback);
        }
        else
        {
            // leaving entry at the last index
            currentVoiceOverIndex = voiceOvers.Length - 1;
        }
    }

    /// <summary>
    /// Gos to prevoius voice over.
    /// </summary>
    public void GoToPrevoiusVoiceOver(UnityAction callback=null)
    {
        --currentVoiceOverIndex;

        if(currentVoiceOverIndex > -1)
        {
            SetVoiceOver(currentVoiceOverIndex, callback);
        }
        else
        {
            // leaving entry at the first index
            currentVoiceOverIndex = 0;
        }
    }

    /// <summary>
    /// Sets the voice over.
    /// </summary>
    /// <param name="index">Index.</param>
    private void SetVoiceOver(int index, UnityAction callback=null, float startDelay = 0.0f)
    {
        currentVoiceOverIndex = index;
        AudioManager.Instance.SetVoiceOver(voiceOvers[currentVoiceOverIndex], callback, startDelay);
    }

    #endregion


    #region ILocalizedObject implementation
    public void OnLanguageChanged(string isoCode)
    {
        Debug.Log(isoCode);
        if(isoCode == "bra")
        {
            isoCode = "pt";
        }

        // --- Voice Overs ---
        voiceOvers = Resources.LoadAll<AudioClip>("Audio_VoiceOver/tutorial/" + isoCode + "");
        
        // Fallback
        if(voiceOvers == null || voiceOvers.Length <= 0)
        {
            voiceOvers = Resources.LoadAll<AudioClip>("Audio_VoiceOver/tutorial/" + default_iso + "");
        }

        // --- Colors ---
        voiceOversColor = new AudioClip[4];

        voiceOversColor[0] = Resources.Load<AudioClip>("Audio_VoiceOver/colors/" + isoCode + "/voice_red");
        voiceOversColor[1] = Resources.Load<AudioClip>("Audio_VoiceOver/colors/" + isoCode + "/voice_green");
        voiceOversColor[2] = Resources.Load<AudioClip>("Audio_VoiceOver/colors/" + isoCode + "/voice_yellow");
        voiceOversColor[3] = Resources.Load<AudioClip>("Audio_VoiceOver/colors/" + isoCode + "/voice_lila");

        // Fallback
        if(voiceOversColor[0] == null)
        {
            voiceOversColor[0] = Resources.Load<AudioClip>("Audio_VoiceOver/colors/" + default_iso + "/voice_red");
        }

        if(voiceOversColor[1] == null)
        {
            voiceOversColor[1] = Resources.Load<AudioClip>("Audio_VoiceOver/colors/" + default_iso + "/voice_green");
        }

        if(voiceOversColor[2] == null)
        {
            voiceOversColor[2] = Resources.Load<AudioClip>("Audio_VoiceOver/colors/" + default_iso + "/voice_yellow");
        }

        if(voiceOversColor[3] == null)
        {
            voiceOversColor[3] = Resources.Load<AudioClip>("Audio_VoiceOver/colors/" + default_iso + "/voice_lila");
        }

        // --- Theo ---

        voiceOversTheoAnnouncement = Resources.LoadAll<AudioClip>("Audio_VoiceOver/theo_announcement/" + isoCode + "");

        // Fallback
        if(voiceOversTheoAnnouncement == null || voiceOversTheoAnnouncement.Length <= 0)
        {
            voiceOversTheoAnnouncement = Resources.LoadAll<AudioClip>("Audio_VoiceOver/theo_announcement/" + default_iso + "");
        }
    }
    #endregion

    public void VoiceOverOfColor(FruitColor id)
    {
        showPointerAtColorWheelTutorial = false;

        if(id > 0)
        {
            UnityAction callback = null;

            if(currentFruitsToPickCount >= fruitsToPickCount-1)
            {
                voiceOverOfColorDone = true;
            }

            AudioManager.Instance.SetVoiceOver(voiceOversColor[currentFruitsToPickCount], callback);
            ++currentFruitsToPickCount;
        }
    }
}
