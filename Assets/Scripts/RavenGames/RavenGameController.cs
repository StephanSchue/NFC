using UnityEngine;
using System.Collections;

public delegate void RavenGameCompleteCallback(bool win);

[System.Serializable]
/// <summary>
/// Baseclass for raven minigames
/// </summary>
public abstract class RavenGameController : MonoBehaviour 
{
	// --- Transform Objects ---
	public GameObject mRaven = null;
	public Transform mRavenStartPoint;

	// --- Game Loop Variables ---
	public bool miniGameIsRunning { get; private set; }

	public float timeForTheGame = 5.0f;
	public float timeWhenMinigameEnd { get; private set; }

	protected bool gameWin = false;
	protected RavenGameCompleteCallback gameFinishedCallback;

    protected bool tutorialMode = false;

	#region Game-functions

	/// <summary>
	/// Start the mini game.
	/// </summary>
    public abstract void StartMiniGame(bool tutorialMode=false);

	/// <summary>
	/// Dos the action.
	/// </summary>
    public void StartGameLoop()
	{
		timeWhenMinigameEnd = Time.time + timeForTheGame;
		miniGameIsRunning = true;
        Debug.Log("GameLoop started!");
	}

	/*
	/// <summary>
	/// Called per Frame
	/// </summary>
	private void Update()
	{
		if(!gameIsRunning)
			return;

		if(timeWhenGameEnd > Time.time) 
		{
			// - Check clean up the screen -
			if(Input.GetMouseButton(0)) 
			{
				DoAction();
			}

			// - Check mask already cleaned up -
			if(WinConditionsAreSovled())
			{
				WinTheGame();
			}
		}
		else 
		{
			// - Lose Game -
			LooseTheGame();
		}
	}
	*/

	/// <summary>
	/// Dos the action.
	/// </summary>
	public abstract void DoAction();

	public abstract void ClearAction();

	/// <summary>
	/// Check if the Wincondition of the Game is set
	/// </summary>
	/// <returns><c>true</c>, if conditions are sovled was windowed, <c>false</c> otherwise.</returns>
	public abstract bool WinConditionsAreSovled();

	#endregion
		
	#region Utilities

	/// <summary>
	/// Move Transform to destination.
	/// </summary>
	public void MoveToDestination(GameObject movedObject, Vector3 destination, string onComplete, float time=0.2f, float delay = 0.0f)
	{
		iTween.MoveTo(movedObject, iTween.Hash(
			"x", destination.x,
			"y", destination.y,
			"z", destination.z,
			"time", time,
			"delay", delay,
			"easetype", "easeInOutQuad",
			"onComplete", onComplete,
			"onCompleteTarget", gameObject
		));
	}
		
	#endregion

	#region Win/Loose

	/// <summary>
	/// If the player win the game.
	/// </summary>
	public virtual void WinTheGame(RavenGameCompleteCallback callback)
	{
		gameFinishedCallback = callback;
		miniGameIsRunning = false;
		gameWin = true;
		Debug.Log("END: Win");
	}

	/// <summary>
	/// If the player Looses the game.
	/// </summary>
	public virtual void LooseTheGame(RavenGameCompleteCallback callback)
	{
		gameFinishedCallback = callback;
		miniGameIsRunning = false;
		gameWin = false;
		Debug.Log("END: Lose");
	}

	/// <summary>
	/// Clean the hole minigame scene
	/// </summary>
	public abstract void CleanUpScreen();


	public void ShowRaven()
	{
		mRaven.SetActive(true);
	}

	#endregion
}
