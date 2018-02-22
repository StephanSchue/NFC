using UnityEngine;
using System.Collections;

[System.Serializable]
/// <summary>
/// Raven Game One - Cut the Rope in time
/// </summary>
public class RavenGame2Controller : RavenGameController
{
	[Header("Specific Preferences")]

    public GameController gameController;

	// --- Transform Objects ---
	public GameObject mRopePrefab;
	public string mRavenSittingPostionTagName = "RavenFishingLotePos";
	private GameObject mTreePostion;
	public GameObject mRope;

    public GameObject cutZone;
    public BasketController basketController;

    public float shakeDelay = 0.5f;

    public float fruitDropDelayTime = 2.0f;
    private float nextTimeFruitDrop = 0.0f;

	// --- CutTheRope component Settings ---
	public CutTheRopeController mCutTheRopeController;
    

	#region Game-functions
	/// <summary>
	/// Start the mini game.
	/// </summary>
    public override void StartMiniGame(bool tutorialMode=false)
	{
        this.tutorialMode = tutorialMode;

        /*
		if(mTreePostion == null)
			mTreePostion = GameObject.FindGameObjectWithTag(mRavenSittingPostionTagName);
        */

		mCutTheRopeController.Initalize();
		mCutTheRopeController.gameObject.SetActive(false);
		mCutTheRopeController.ResetPosition();

		//MoveToTheSky();
        ShowTheFishingPole();

        mRaven.GetComponent<RavenController>().OnFishingLoteGrab += GrapFruitFromBasket;
	}

    protected void GrapFruitFromBasket()
    {
        basketController.RealShake(Vector3.forward * 5, 0.5f);

        if(Time.time < nextTimeFruitDrop)
            return;

        FruitController lastFruit = basketController.GetLastFruit();

        if(lastFruit != null)
        {
            gameController.RemoveFruitFromTheBasket(lastFruit.iD);
            basketController.RemoveFuit(lastFruit);
        }

        nextTimeFruitDrop = Time.time + fruitDropDelayTime;
    }

	/// <summary>
	/// Move the raven to the stone
	/// </summary>
	public void MoveToTheSky()
	{
		//mRaven.GetComponent<RavenController>().Fly(ShowTheFishingPole);
        mRaven.GetComponent<RavenController>().FlyDown(ShowTheFishingPole);
		MoveToDestination(mRaven.gameObject, mTreePostion.transform.position, "FlyingToTheGround", 0.1f);
	}

    public void ShowTheFishingPole()
	{
        mRaven.GetComponent<RavenController>().StartFishing(ShowRope);
        nextTimeFruitDrop = Time.time + (fruitDropDelayTime * 2.5f);
	}

	/// <summary>
	/// Shows the fishing pole.
	/// </summary>
	public void ShowRope()
	{
        cutZone.transform.position = mRaven.transform.position;
        cutZone.SetActive(true);
        basketController.Shake(basketController.shakeIntensity, timeForTheGame, shakeDelay);

        /*
		// Show the Lute
		mRope = Instantiate<GameObject>(mRopePrefab);
		mRope.SetActive(true);
        */

	    mCutTheRopeController.gameObject.SetActive(true);

		// Start the Game
		base.StartGameLoop();
	}

	/// <summary>
	/// Start the CutTheRopeController
	/// </summary>
	public override void DoAction()
	{
		mCutTheRopeController.Cut();
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
		return mCutTheRopeController.cutCount > 0;
	}

	/// <summary>
	/// Cleans up screen.
	/// </summary>
	public override void CleanUpScreen()
	{
		Destroy(mRope);
		mCutTheRopeController.ResetPosition();
		mCutTheRopeController.gameObject.SetActive(false);
		cutZone.SetActive(false);

		//mRaven.transform.position = mRavenStartPoint.transform.position;

		if(gameFinishedCallback != null)
		{
			gameFinishedCallback(gameWin);
			gameFinishedCallback = null;
		}
	}

	public void SneekRaven()
	{
		//mRaven.GetComponent<RavenController>().PoutAndStomp(CleanUpScreen);
		//MoveToDestination(mRaven.gameObject, mTreePostion.transform.position - (Vector3.up * 18.0f), "CleanUpScreen", 3.0f);
		//MoveToDestination(mRaven.gameObject, mRavenStartPoint.transform.position, "CleanUpScreen", 2.0f);
	}

    public void StopFishing()
    {
        basketController.StopShake();
		mRaven.GetComponent<RavenController>().StopFishing(CleanUpScreen);
    }

	#endregion

	#region Win/Loose

	/// <summary>
	/// If the player win the game.
	/// </summary>
	public override void WinTheGame(RavenGameCompleteCallback callback)
	{
		base.WinTheGame(callback);
        StopFishing();
	}

	/// <summary>
	/// If the player Looses the game.
	/// </summary>
	public override void LooseTheGame(RavenGameCompleteCallback callback)
	{
		base.LooseTheGame(callback);
        StopFishing();
	}

	#endregion
}
