using UnityEngine;
using System.Collections;

[System.Serializable]
/// <summary>
/// Raven Game One - Shooting Bubble on Screen
/// </summary>
public class RavenGame1Controller : RavenGameController 
{
	[Header("Specific Preferences")]

	// --- Transform Objects ---
	public Transform mShootingPosition;
	public GameObject mStone;
	public GameObject mBubbleInstance;
	public Transform mBubbleDestination;
    public Animator HintHand;
    public Transform mBallon;

    public Texture2D[] splashes;
    public Material[] ballonColors;

	// --- Screen cleaner component Settings ---
	public float bubbleShootSpeed = 0.2f;
	public float bubbleShootDelay = 2.0f;

	public BubbleDrawer screenCleaner;
	public float bubbleFadeOutTime = 3.0f;

	[Range(0.0f, 1.0f)]
    public float percentageOfCleanUp = 0.75f;

    private bool useHintHand = false;
    
	#region Game functions

	/// <summary>
	/// Start the mini game.
	/// </summary>
    public override void StartMiniGame(bool tutorialMode=false)
	{
        this.tutorialMode = tutorialMode;

        // Choose Random Ballon
        int random = Random.Range(0, ballonColors.Length);
        screenCleaner.SetSplashTexture(random);     
        mBallon.GetComponent<MeshRenderer>().material = ballonColors[random];

        // Start Moving
		MoveToTheStone();
	}

	/// <summary>
	/// Move the raven to the stone
	/// </summary>
	public void MoveToTheStone()
	{
		mStone.SetActive(true);
		mBubbleInstance.SetActive(false);

		mRaven.GetComponent<RavenController>().Appear(OnReachDestination);
		//MoveToDestination(mRaven.gameObject, mStone.transform.position, "OnReachDestination", 2.0f);
	}
	
	/// <summary>
	/// Callback if the object reach the destination.
	/// </summary>
	private void OnReachDestination()
	{
		// Shooting BlueBubble
        mBallon.transform.position = mShootingPosition.position;
        mBallon.parent = mShootingPosition;
        mBallon.gameObject.SetActive(true);

		RavenThrow();
	}

	private void RavenThrow()
	{
		mRaven.GetComponent<RavenController>().Throw(ShootBubble);
	}

	/// <summary>
	/// Shoot the bubble.
	/// </summary>
	private void ShootBubble()
	{
        mBallon.gameObject.SetActive(true);
        mBallon.parent = null;
        MoveToDestination(mBallon.gameObject, mBubbleDestination.position + (Vector3.up * 0.2f), "OnBubbleIsAtDestination", bubbleShootSpeed, bubbleShootDelay);
	}

	/// <summary>
	/// Callback if bubble is on destination
	/// </summary>
	private void OnBubbleIsAtDestination()
	{
        mBallon.gameObject.SetActive(false);
        mBubbleInstance.transform.position = mBallon.position - (Vector3.up * 0.2f);
        mBubbleInstance.SetActive(true);

		base.StartGameLoop();

        if(tutorialMode)
        {
            EnableHintHand();
            StartCoroutine(DoFunctionWithDelay(PlayHintHandAnimation, 2.0f));
        }
	}

	/// <summary>
	/// Cleans up screen.
	/// </summary>
	public void BubbleDisabled()
	{
        DisableHintHand();

		iTween.ValueTo(gameObject, iTween.Hash(
			"from", 1.0f,
			"to", 0.0f,
			"time", bubbleFadeOutTime, 
			"easetype", "linear",
			"onupdate", "SetAlpha",
			"onComplete", "CleanUpScreen",
			"onCompleteTarget", gameObject
		));
            
		//MoveToDestination(mRaven.gameObject, mRavenStartPoint.transform.position, "CleanUpScreen", 2.0f);
	}

	public void SetAlpha(float newAlpha) 
	{
		screenCleaner.SetAlpha(newAlpha);
	}

	/// <summary>
	/// Cleans up the screen.
	/// </summary>
	public override void CleanUpScreen()
	{
		// Clean Up the Screen
		mBubbleInstance.transform.position = mStone.transform.position;
		mBubbleInstance.SetActive(false);
		screenCleaner.ResetMask();
		screenCleaner.SetAlpha(1.0f);

		// Let the Raven move back
		mStone.SetActive(false);

		if(gameFinishedCallback != null)
		{
			gameFinishedCallback(gameWin);
			gameFinishedCallback = null;
		}
	}
		
	/// <summary>
	/// Start the CutTheRopeController
	/// </summary>
	public override void DoAction()
	{
		screenCleaner.DrawStencial();

        if(useHintHand)
            DisableHintHand();
	}

	public override void ClearAction()
	{

	}

	/// <summary>
	/// Check if the Wincondition of the Game is set
	/// </summary>
	/// <returns>true</returns>
	/// <c>false</c>
	public override bool WinConditionsAreSovled()
	{
		return screenCleaner.MaskedPercentage() >= percentageOfCleanUp;
	}

	#endregion

	#region Win/Loose

	/// <summary>
	/// If the player win the game.
	/// </summary>
	public override void WinTheGame(RavenGameCompleteCallback callback)
	{
		base.WinTheGame(callback);
		BubbleDisabled();
	}

	/// <summary>
	/// If the player Looses the game.
	/// </summary>
	public override void LooseTheGame(RavenGameCompleteCallback callback)
	{
		base.LooseTheGame(callback);
		BubbleDisabled();
	}

	#endregion

    public void EnableHintHand()
    {
        useHintHand = true;
    }

    public void DisableHintHand()
    {
        useHintHand = false;
        HintHand.gameObject.SetActive(false);
    }

    private void PlayHintHandAnimation()
    {
        if(!useHintHand)
            return;

        HintHand.gameObject.SetActive(true);
        HintHand.SetTrigger("PlayBubbleSwipe");
    }

    private IEnumerator DoFunctionWithDelay(FruitHighlightCallback method, float delay)
    {
        yield return new WaitForSeconds(delay);
        method();
    }
}
