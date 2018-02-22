using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class DiceController : MonoBehaviour 
{
	public GameController gameController = null;

	public FruitColor[] imageColorList;

	public float likelihoodOfRaven;
	public float likelihoodOfRed;
	public float likelihoodOfYellow;
	public float likelihoodOfBlue;
	public float likelihoodOfGreen;

	private float curLikelihoodOfRaven;
	private float curLikelihoodOfRed;
	private float curLikelihoodOfYellow;
	private float curLikelihoodOfBlue;
	private float curLikelihoodOfGreen;

	public SpriteRenderer diceBackground;
	public BoxCollider2D diceBackgroundCollider;

	public Color diceBackgroundColor;

	private GameObject diceModelObj;
	public bool canRollDice = true;
	private DiceFieldType curDiceId;

	private bool ravenSetLastRound = false;
	public float diceRollDuration = 1f;

	// --- Swipe Detection ---
	private Vector2 startSwipePosition;
	private float startSwipeTime;
	private bool swiped = false;

	// ----------

	private void Awake()
	{
		// TODO: EventSystem
		// Assign the game conrtoller
		GameObject gameControllerObj = GameObject.Find("GameController");

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
		// Get the model of the dice (cube)
		diceModelObj = GameObject.Find("DiceObject");

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
		curLikelihoodOfRed = likelihoodOfRed;
		curLikelihoodOfYellow = likelihoodOfYellow;
		curLikelihoodOfBlue = likelihoodOfBlue;
		curLikelihoodOfGreen = likelihoodOfGreen;
	}

	/// <summary>
	/// Actions done once per Frame
	/// </summary>
	private void Update()
	{
		// -- Role Dice --
		if(canRollDice && (Input.acceleration.magnitude > 2f || swiped))
		{
			swiped = false;

			Enable(true);
			RollDice();
		}

		// -- Swipe Stuff --
		if (canRollDice && Input.touchCount > 0) 
		{
			if (Input.GetTouch (0).phase == TouchPhase.Began) 
			{
				startSwipePosition = Input.GetTouch(0).position;
			}
			else if (Input.GetTouch(0).phase == TouchPhase.Ended) 
			{
				Vector2 heading = Input.GetTouch(0).position - startSwipePosition;
				float distance = heading.magnitude;
				Vector2 direction = heading / distance; // This is now the normalized direction.
				float normalizedDistance = (distance / Screen.width);

				if (normalizedDistance > 0.1f && direction != Vector2.zero && direction.x > 0.5f) 
				{
					swiped = true;
				}
			}
		}
	}

	/// <summary>
	/// Role the dice
	/// </summary>
	public void RollDice()
	{
		// Check if a game is active and block a dice roll before the last game ends
		if (canRollDice) 
		{
			diceModelObj.transform.rotation = Quaternion.identity;

			// -- Initialize --
			diceBackground.enabled = diceBackgroundCollider.enabled = true;

			// Deactivate the dice temporarilly
			AllowDiceToRoll(false);

			// Check current fruit count to set out the possiblity
			if(gameController.GetSpecificFruitCount(gameController.GetFruitColorToDiceNumber(DiceFieldType.One)) <= 0)
			{
				curLikelihoodOfRed = 0;
			}

			if(gameController.GetSpecificFruitCount(gameController.GetFruitColorToDiceNumber(DiceFieldType.Two)) <= 0)
			{
				curLikelihoodOfYellow = 0;
			}

			if(gameController.GetSpecificFruitCount(gameController.GetFruitColorToDiceNumber(DiceFieldType.Three)) <= 0)
			{
				curLikelihoodOfBlue = 0;
			}

			if(gameController.GetSpecificFruitCount(gameController.GetFruitColorToDiceNumber(DiceFieldType.Four)) <= 0)
			{
				curLikelihoodOfGreen = 0;
			}

			// -- 1. Calculate the total amount of posibilities --
			float totalAmountOfPosibilities = curLikelihoodOfRaven + curLikelihoodOfRed + curLikelihoodOfYellow + curLikelihoodOfBlue + curLikelihoodOfGreen;
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
			if (randomNumber < curLikelihoodOfRaven) {
				// Tell the GameController what we are looking for (-1 is for crow)
				curDiceId = DiceFieldType.Raven;
				ravenSetLastRound = true;

				// Rotating dice animation
				DiceAnimation (720, 720);
			} else if (randomNumber >= curLikelihoodOfRaven && randomNumber < (curLikelihoodOfRaven + curLikelihoodOfRed)) {
				// Tell the GameController what we are looking for
				curDiceId = DiceFieldType.One;

				// Rotating dice animation
				DiceAnimation (810, 720);
			} else if (randomNumber >= (curLikelihoodOfRaven + curLikelihoodOfRed) && randomNumber < (curLikelihoodOfRaven + curLikelihoodOfRed + curLikelihoodOfYellow)) {
				// Tell the GameController what we are looking for
				curDiceId = DiceFieldType.Two;

				// Rotating dice animation
				DiceAnimation (720, 630);
			} else if (randomNumber >= (curLikelihoodOfRaven + curLikelihoodOfRed + curLikelihoodOfYellow) && randomNumber < (curLikelihoodOfRaven + curLikelihoodOfRed + curLikelihoodOfYellow + curLikelihoodOfBlue)) {
				// Tell the GameController what we are looking for
				curDiceId = DiceFieldType.Three;

				// Rotating dice animation
				DiceAnimation (720, 90);
			} else if (randomNumber >= (curLikelihoodOfRaven + curLikelihoodOfRed + curLikelihoodOfYellow + curLikelihoodOfBlue)) {
				// Tell the GameController what we are looking for
				curDiceId = DiceFieldType.Four;

				// Rotating dice animation
				DiceAnimation (270, 720);
			} else {
				Debug.Log("WTF: " + randomNumber + " - " + totalAmountOfPosibilities);
			}
		}

	}

	/// <summary>
	/// Plays the Animaton of the dice
	/// </summary>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	private void DiceAnimation(int x, int y)
	{
		iTween.RotateTo(diceModelObj,iTween.Hash(
			"x"   , x,
			"y"   , y,
			"z"   , 720,
			"time", diceRollDuration,
			"easetype", "easeInOutQuad",
			"onComplete", "DiceRoleComplete",
			"onCompleteTarget", gameObject
		));
	}

	/// <summary>
	/// Allows the dice to roll.
	/// </summary>
	public void AllowDiceToRoll(bool status)
	{
		canRollDice = status;
		gameController.DebugText.text = status.ToString();
	}

	/// <summary>
	/// Dice role complete callback
	/// </summary>
	void DiceRoleComplete()
	{
		diceBackground.enabled = diceBackgroundCollider.enabled = false;
		gameController.SetDiceResult(curDiceId);
	}

	/// <summary>
	/// Enable the dice
	/// </summary>
	/// <param name="status">If set to <c>true</c> status.</param>
	public void Enable(bool status)
	{
		MeshRenderer[] meshR = GetComponentsInChildren<MeshRenderer>();

		for (int i = 0; i < meshR.Length; i++) 
		{
			meshR[i].enabled = status;
		}

		meshR = null;
	}
}