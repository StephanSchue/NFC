using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ColorWheelController : MonoBehaviour 
{
	public DiceFieldType[] diceFields;
	public Sprite[] sprites;
	public Image[] imageObjects;
    public Image[] shadowObjects;

	private Animator colorWheel;
	private bool colorWheelHide = true;

	public iTween.EaseType easeType = iTween.EaseType.linear;
	public bool useTime = true;

	// --- Value likelyhood ---
	[HideInInspector()]
	public float likelihoodOfRaven;
	[HideInInspector()]
	public float likelihoodOfOne;
	[HideInInspector()]
	public float likelihoodOfTwo;
	[HideInInspector()]
	public float likelihoodOfThree;
	[HideInInspector()]
	public float likelihoodOfFour;

	private float curLikelihoodOfRaven;
	private float curLikelihoodOfOne;
	private float curLikelihoodOfTwo;
	private float curLikelihoodOfThree;
	private float curLikelihoodOfFour;

	public UIController uiController;
	public RectTransform colorWheelTransform;
	private GameController gameController;
	public Image background;

	private DiceFieldType curDiceValue;

	private bool blockRoll = false;
	public float delay = 0.2f;

    public Image wheelGlow;

	public Animator hintHand;

	// ---
	public Animator colorWheelArrow;
	public const float arrowBlinkingDelayTime = 5.0f;
	private Coroutine blinkingCoroutine;
	private bool ravenSetLastRound = false;

	[Header("Rotation")]
	public float minRotation = 4;
	public float maxRotation = 10;

	[Header("Time")]
	public float time = 1.0f;

	[Header("Speed")]
	public float minSpeed = 5.0f;
	public float maxSpeed = 10.0f;

    [Header("SFX")]
    public AudioClip WheelMovesDown;
    public AudioClip WheelTurn;
    public AudioClip WheelStops;
    public AudioClip WheelShowsColorSound;
    public AudioClip WheelShowsRavenSound;

	private void Awake()
	{
		GameObject gameControllerObj = GameObject.Find("GameController");
		colorWheel = GetComponent<Animator>();

		if(gameControllerObj)
		{
			gameController = gameControllerObj.GetComponent<GameController>();
		}
	}

	/// <summary>
	/// Inizalizing
	/// </summary>
	private void Start()
	{
		// Initalize the dice controlls
		Initialize();
	}

	/// <summary>
	/// Initialize the dice.
	/// </summary>
	public void Initialize()
	{
		// Don't start with the raven
		ravenSetLastRound = true;

		// Set Init ColorPossiblity
		curLikelihoodOfRaven = likelihoodOfRaven;
		curLikelihoodOfOne = likelihoodOfOne;
		curLikelihoodOfTwo = likelihoodOfTwo;
		curLikelihoodOfThree = likelihoodOfThree;
		curLikelihoodOfFour = likelihoodOfFour;

		StartBlinking();
	}

	public void PrepairColorWheel()
	{
		for (int i = 0; i < imageObjects.Length; i++) 
		{
			if(i < diceFields.Length)
			{
				imageObjects[i].sprite = sprites[i];
			}
		}
	}

    public void Roll(bool directionRight=true)
	{
		if (blockRoll)
			return;

		curDiceValue = CalculateDice();
        Roll(curDiceValue, directionRight);
	}
        
    public void Roll(int diceType, bool directionRight=true)
	{
		if (blockRoll)
			return;

        UnHightLightSelected();
		curDiceValue = (DiceFieldType)diceType;
        Roll(curDiceValue, directionRight);
	}

    public void RollRaven()
    {
        if (blockRoll)
            return;

        UnHightLightSelected();
        curDiceValue = DiceFieldType.Raven;
        Roll(curDiceValue, true);
    }

	public bool IsRollAllowed()
	{
		return !blockRoll;
	}

    /// <summary>
    /// Main Roll Function
    /// </summary>
    /// <param name="diceType">Dice type.</param>
    /// <param name="directionRight">If set to <c>true</c> direction right.</param>
    public void Roll(DiceFieldType diceType, bool directionRight=true)
	{
		curDiceValue = diceType;
        int direction = directionRight ? -1 : 1;
		int random = Random.Range((int)minRotation, (int)maxRotation);

		switch (diceType) 
		{
		case DiceFieldType.Raven:
			Animation(GetOneRound(random));
			break;
		case DiceFieldType.One:
                Animation(GetOneRound(random * direction + 0.2f));
			break;
		case DiceFieldType.Two:
                Animation(GetOneRound(random * direction+ 0.4f));
			break;
		case DiceFieldType.Three:
                Animation(GetOneRound(random * direction+ 0.6f));
			break;
		case DiceFieldType.Four:
                Animation(GetOneRound(random * direction+ 0.8f));
			break;
		default:
			break;
		}
        
        PlayWheelTurn();
	}

	/// <summary>
	/// Plays the Animaton of the dice
	/// </summary>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	private void Animation(int angle)
	{
        uiController.FadeOut();

		if(useTime) 
		{
			iTween.RotateTo (colorWheelTransform.gameObject, iTween.Hash (
				"z", angle,
				"time", time,
				"delay", delay,
				"easetype", easeType,
				"onComplete", "AnimationFinished",
				"onCompleteTarget", gameObject
			));
		} 
		else 
		{
			float random = Random.Range(minSpeed, maxSpeed);

			iTween.RotateTo (colorWheelTransform.gameObject, iTween.Hash (
				"z", angle,
				"speed", random * 100,
				"delay", delay,
				"easetype", easeType,
				"onComplete", "AnimationFinished",
				"onCompleteTarget", gameObject
			));
		}
	}

	public int GetOneRound(float count = 1.0f)
	{
		return (int)(360 * count);
	}

	private void AnimationFinished()
	{
		uiController.FadeIn(RollIsFinished);
	}

	public void AllowToRoll(bool status)
	{
		blockRoll = !status;
	}

	public void RollIsFinished()
	{
		gameController.SetDiceResult(curDiceValue);
        PlayWheelStops(curDiceValue);
	}

	public void Enable(bool status)
	{
		if(status)
		{
			ShowColorWheel();
		}
		else
		{
			HideColorWheel();
		}
	}

    public void HightLightSelected(DiceFieldType piece)
    {
        Animator mainAnimator = null;
        Animator animator = null;
        //currentSelectedShadow.enabled = true;

        for (int i = 0; i < diceFields.Length; i++)
        {
            if(piece == diceFields[i])
            {
                //imageObjects[i].material.SetFloat("_Sat", 0.0f);
                shadowObjects[i].enabled = true;
                mainAnimator = imageObjects[i].GetComponent<Animator>();
                animator = shadowObjects[i].GetComponent<Animator>();

                if (animator != null)
                {
                    mainAnimator.SetTrigger("Highlight");
                    animator.SetTrigger("Highlight");
                }

                break;
            }
        }
    }

    public void UnHightLightSelected()
    {
        Animator mainAnimator = null;
        Animator animator = null;
        //currentSelectedShadow.enabled = false;

        for (int i = 0; i < diceFields.Length; i++)
        {
            //imageObjects[i].material.SetFloat("_Sat", 1.0f);
            shadowObjects[i].enabled = false;
            mainAnimator = imageObjects[i].GetComponent<Animator>();
            animator = shadowObjects[i].GetComponent<Animator>();

            if (animator != null)
            {
                mainAnimator.SetTrigger("UnHighlight");
                animator.SetTrigger("UnHightlight");
            }
        }
    }

	public DiceFieldType CalculateDice()
	{
		// -- Initialize --
		uiController.FadeOut();

		// Deactivate the dice temporarilly
		AllowToRoll(false);

		// Check current fruit count to set out the possiblity
		//Debug.Log(gameController.GetFruitColorToDiceNumber(DiceFieldType.One) + " = " + gameController.GetSpecificFruitCount(gameController.GetFruitColorToDiceNumber(DiceFieldType.One)));

		if(gameController.GetSpecificFruitCount(gameController.GetFruitColorToDiceNumber(DiceFieldType.One)) <= 0)
		{
			curLikelihoodOfOne = 0;
		}
			
		//Debug.Log(gameController.GetFruitColorToDiceNumber(DiceFieldType.Two) + " = " + gameController.GetSpecificFruitCount(gameController.GetFruitColorToDiceNumber(DiceFieldType.Two)));

		if(gameController.GetSpecificFruitCount(gameController.GetFruitColorToDiceNumber(DiceFieldType.Two)) <= 0)
		{
			curLikelihoodOfTwo = 0;
		}
			
		//Debug.Log(gameController.GetFruitColorToDiceNumber(DiceFieldType.Three) + " = " + gameController.GetSpecificFruitCount(gameController.GetFruitColorToDiceNumber(DiceFieldType.Three)));

		if(gameController.GetSpecificFruitCount(gameController.GetFruitColorToDiceNumber(DiceFieldType.Three)) <= 0)
		{
			curLikelihoodOfThree = 0;
		}

		//Debug.Log(gameController.GetFruitColorToDiceNumber(DiceFieldType.Four) + " = " + gameController.GetSpecificFruitCount(gameController.GetFruitColorToDiceNumber(DiceFieldType.Four)));

		if(gameController.GetSpecificFruitCount(gameController.GetFruitColorToDiceNumber(DiceFieldType.Four)) <= 0)
		{
			curLikelihoodOfFour = 0;
		}

		// -- 1. Calculate the total amount of posibilities --
		float totalAmountOfPosibilities = curLikelihoodOfRaven + curLikelihoodOfOne + curLikelihoodOfTwo + curLikelihoodOfThree + curLikelihoodOfFour;
		float randomNumber = 0.0f;

		if(ravenSetLastRound)
		{
			// Chance without raven
			randomNumber = Random.Range(curLikelihoodOfRaven, totalAmountOfPosibilities);
			ravenSetLastRound = false;
		}
		else
		{
			// Normal Chance
			randomNumber = Random.Range(0, totalAmountOfPosibilities);
		}

		// -- 2. Set Dice Result & Start Play Animation --
		if (randomNumber < curLikelihoodOfRaven) 
		{
			// Tell the GameController what we are looking for (-1 is for crow)
			curDiceValue = DiceFieldType.Raven;
			ravenSetLastRound = true;
		} 
        else if(curLikelihoodOfOne > 0 && randomNumber >= curLikelihoodOfRaven && randomNumber < (curLikelihoodOfRaven + curLikelihoodOfOne)) 
		{
			// Tell the GameController what we are looking for
			curDiceValue = DiceFieldType.One;
		} 
        else if(curLikelihoodOfTwo > 0 && randomNumber >= (curLikelihoodOfRaven + curLikelihoodOfOne) && randomNumber < (curLikelihoodOfRaven + curLikelihoodOfOne + curLikelihoodOfTwo))
		{
			// Tell the GameController what we are looking for
			curDiceValue = DiceFieldType.Two;
		} 
        else if(curLikelihoodOfThree > 0 && randomNumber >= (curLikelihoodOfRaven + curLikelihoodOfOne + curLikelihoodOfTwo) && randomNumber < (curLikelihoodOfRaven + curLikelihoodOfOne + curLikelihoodOfTwo + curLikelihoodOfThree)) 
		{
			// Tell the GameController what we are looking for
			curDiceValue = DiceFieldType.Three;
		} 
        else if(curLikelihoodOfFour > 0 && randomNumber >= (curLikelihoodOfRaven + curLikelihoodOfOne + curLikelihoodOfTwo + curLikelihoodOfThree)) 
		{
			// Tell the GameController what we are looking for
			curDiceValue = DiceFieldType.Four;
		} 
		else 
		{
            if(curLikelihoodOfOne > 0)
            {
                curDiceValue = DiceFieldType.One;
            }
            else if(curLikelihoodOfTwo > 0)
            {
                curDiceValue = DiceFieldType.Two;
            }
            else if(curLikelihoodOfThree > 0)
            {
                curDiceValue = DiceFieldType.Three;
            }
            else if(curLikelihoodOfFour > 0)
            {
                curDiceValue = DiceFieldType.Four;
            }
            else
            {  
			    Debug.Log("WTF: " + randomNumber + " - " + totalAmountOfPosibilities);
            }
		}

		return curDiceValue;
	}

	public void HideColorWheel()
	{
		if(!colorWheelHide) 
		{
			colorWheel.SetTrigger("Up");
			colorWheelHide = true;
            PlayWheelMovesDown();
		}
	}

	public void ShowColorWheel()
	{
		if(colorWheelHide) 
		{
			colorWheel.SetTrigger("Down");
			colorWheelHide = false;
            UnHightLightSelected();
            PlayWheelMovesDown();
		}
	}

	public void EnableSwipeHint()
	{
		hintHand.gameObject.SetActive(true);
		hintHand.SetBool("Swipe", true);
	}

	public void DisableSwipeHint()
	{
		hintHand.SetBool("Swipe", false);
		hintHand.gameObject.SetActive(false);
	}

	public bool IsSwipeHindEnabled()
	{
		return hintHand.gameObject.activeSelf;
	}

    public void StartBlinking(float delay = arrowBlinkingDelayTime)
	{
		if(blinkingCoroutine != null)
			StopCoroutine(blinkingCoroutine);
		
        blinkingCoroutine = StartCoroutine(DoAnimationWithDelay(colorWheelArrow, "Blinking", true, delay));
	}

	public void StopBlinking()
	{
		if(blinkingCoroutine != null)
			StopCoroutine(blinkingCoroutine);
		
		blinkingCoroutine = StartCoroutine(DoAnimationWithDelay(colorWheelArrow, "Blinking", false, 0.0f));
	}

	private IEnumerator DoAnimationWithDelay(Animator animator, string variableName, bool status, float delay)
	{
		yield return new WaitForSeconds(delay);
		animator.SetBool(variableName, status);
	}

	public void SetupWheelColors(FruitColor[] colors)
	{
		// Start at the second element (exclude the raven)
		for (int i = 1; i < imageObjects.Length; i++) 
		{
			imageObjects[i].sprite = sprites[(int)colors[i-1]];
		}
	}

    public void HighlightWheel()
    {
        wheelGlow.enabled = true;
        Animator wheelGlowAnimator = wheelGlow.GetComponent<Animator>();

        if(wheelGlowAnimator != null)
        {
            wheelGlowAnimator.SetTrigger("Highlight");
        }
    }

    public void UnHighlightWheel()
    {
        wheelGlow.enabled = false;
        Animator wheelGlowAnimator = wheelGlow.GetComponent<Animator>();

        if(wheelGlowAnimator != null)
        {
            wheelGlowAnimator.SetTrigger("UnHighlight");
        }
    }

    public void OnDestroy()
    {
        UnHightLightSelected();
    }

    #region SFX

    public void PlayWheelMovesDown()
    {
        AudioManager.Instance.SFXChannels[1].loop = false;
        AudioManager.Instance.SetSFXChannel(WheelMovesDown, null, 0, 1);
        //Debug.Log("PlayWheelMovesDown");
    }
        
    public void PlayWheelTurn()
    {
        AudioManager.Instance.SFXChannels[1].loop = true;
        AudioManager.Instance.SetSFXChannel(WheelTurn, null, 0, 1);
        //Debug.Log("PlayWheelTurn");
    }

    public void PlayWheelStops(DiceFieldType diceFieldType)
    {
        AudioManager.Instance.SFXChannels[1].loop = false;

        if(diceFieldType == DiceFieldType.Raven)
        {
            AudioManager.Instance.SetSFXChannel(WheelStops, PlayWheelShowsRavenSound, 0, 1);
        }
        else
        {
            AudioManager.Instance.SetSFXChannel(WheelStops, PlayWheelShowsColorSound, 0, 1);
        }

        Debug.Log("PlayWheelStops");
    }

    public void PlayWheelShowsColorSound()
    {
        AudioManager.Instance.SetSFXChannel(WheelShowsColorSound, null, 0, 1);
        Debug.Log("PlayWheelShowsColorSound");
    }

    public void PlayWheelShowsRavenSound()
    {
        AudioManager.Instance.SetSFXChannel(WheelShowsRavenSound, null, 0, 1);
        Debug.Log("PlayWheelShowsRavenSound");
    }

    #endregion
}
