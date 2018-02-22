using UnityEngine;
using System.Collections;

[System.Serializable]
/// <summary>
/// Raven Game Three - Helicopter
/// </summary>
public class RavenGame3Controller : RavenGameController
{
	[Header("Specific Preferences")]

	// --- Transform Objects ---
	public HelicopterController helicopter;
    public Vector3 helicopterStartingPositionOffset = Vector3.zero;
    public LayerMask helicopterLayer;

    public Transform[] helicopterPath;
    private RavenController mRavenController;

    public Transform helicopterCrashPosition;
    public UIController uiController;

    public CameraController cameraController;
    public Vector3 screenShakeStrength = Vector3.one;
    public float screenShakeTime = 2.0f;

    public BubbleDrawer bubbleDrawer;
    public float percentageOfCleanUp = 0.9f;
    private bool glassFixed = false;

    [Header("SFX")]

    public AudioClip startUpSFX;
    public AudioClip flyingSFX;
    public AudioClip crashSFX;

	#region General-functions
	/// <summary>
	/// Start the mini game.
	/// </summary>
    public override void StartMiniGame(bool tutorialMode=false)
	{
        this.tutorialMode = tutorialMode;
        glassFixed = false;

        // Initalize Raven
        mRavenController = mRaven.GetComponent<RavenController>();
        mRavenController.StartRemote(null);

        // Initalize Helicopter
        helicopter.transform.position = mRaven.transform.position + helicopterStartingPositionOffset;
		SetHelicopterActive(true);

        // Startup Audio
        PlayStartUpSFX();

        // Helicopter flyes in
        helicopter.FlyTo(helicopterPath, HelicopterAtDestination, 10.0f, 0.2f);

        // -- brokenglass cleaner -- 
        bubbleDrawer.SetSplashTexture(0);
         
        base.StartGameLoop();
	}

	/// <summary>
	/// Start the CutTheRopeController
	/// </summary>
	public override void DoAction()
	{
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if(Physics.Raycast(ray, out hit, 100.0f, helicopterLayer))
        {
            StartHelicopterCrash();
        }
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
        return !helicopter.isActiveAndEnabled;
	}

	/// <summary>
	/// Cleans up screen.
	/// </summary>
	public override void CleanUpScreen()
	{
		SetHelicopterActive(false);
        //uiController.FadeOutBrokenGlass(DisableBubble);
        mRavenController.StopRemote(null);


        if(gameFinishedCallback != null)
        {
            gameFinishedCallback(gameWin);
            gameFinishedCallback = null;
        }
	}
		
    public void DisableBubble()
    {
        bubbleDrawer.meshRenderer.enabled = false;
        bubbleDrawer.meshCollider.enabled = false;
    }

	#endregion

	#region Game specific fuctons

	public void SetHelicopterActive(bool status)
	{
		helicopter.gameObject.SetActive(status);
		helicopter.enabled = status;
	}

    public void HelicopterAtDestination()
    {
        
    }

    public void StartHelicopterCrash()
    {
        helicopter.StartHelicopterCrash(helicopterCrashPosition, BreakGlass);
        PlayFlyingSFX();
    }

    public void BreakGlass()
    {
        PlayCrashSFX();

        //uiController.ShowBrokenGlass();
        helicopter.BlackHarkDown();
        cameraController.Shake(screenShakeStrength, screenShakeTime);

        // Start second game
        Debug.Log("Start second game");
        bubbleDrawer.meshRenderer.enabled = true;
        bubbleDrawer.meshCollider.enabled = true;
        StartCoroutine(FixBrokenGlass());

        mRavenController.Poke01(null);
    }

    private IEnumerator FixBrokenGlass()
    {
        while(bubbleDrawer.MaskedPercentage() <= percentageOfCleanUp)
        {
            if(Input.GetMouseButton(0))
            {
                bubbleDrawer.DrawStencial();
            }

            yield return new WaitForEndOfFrame();
        }
            
        glassFixed = true;
        FadeOutGlass();
    }

    private void FadeOutGlass()
    {
        iTween.ValueTo(gameObject, iTween.Hash(
            "from", 1.0f,
            "to", 0.0f,
            "time", 1.0f,
            "delay", 0.0f,
            "easetype", "linear",
            "onComplete", "CleanUpScreen",
            "onCompleteTarget", gameObject,
            "onUpdate", "FadeGlass",
            "onUpdateTarget", gameObject));
    }

    private void FadeGlass(float value)
    {
        bubbleDrawer.meshRenderer.material.color = new Color(bubbleDrawer.meshRenderer.material.color.r,
                                                            bubbleDrawer.meshRenderer.material.color.g,
                                                            bubbleDrawer.meshRenderer.material.color.b,
                                                            value);
    }

    private IEnumerator DoFunctionWithDelay(FruitHighlightCallback method, float delay)
    {
        yield return new WaitForSeconds(delay);
        method();
    }

	#endregion

	#region Win/Loose

	/// <summary>
	/// If the player win the game.
	/// </summary>
	public override void WinTheGame(RavenGameCompleteCallback callback)
	{
		base.WinTheGame(callback);
	}

	/// <summary>
	/// If the player Looses the game.
	/// </summary>
	public override void LooseTheGame(RavenGameCompleteCallback callback)
	{
		base.LooseTheGame(callback);
        StartHelicopterCrash();
	}

	#endregion

    #region SFX

    public void PlayStartUpSFX()
    {
        AudioManager.Instance.SetSFXChannel(startUpSFX, null, 0, 1);
        AudioManager.Instance.SFXChannels[1].loop = true;
    }

    public void PlayFlyingSFX()
    {
        AudioManager.Instance.SetSFXChannel(flyingSFX, null, 0, 1);
        AudioManager.Instance.SFXChannels[1].loop = true;
    }

    public void PlayCrashSFX()
    {
        AudioManager.Instance.SetSFXChannel(crashSFX, null, 0, 1);
        AudioManager.Instance.SFXChannels[1].loop = false;
    }

    #endregion
}
