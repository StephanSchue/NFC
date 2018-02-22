using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public delegate void ScreenFadeCallBack();

public class UIController : MonoBehaviour, IStartscreenMessageTarget
{
	[Header("Videoclips")]
	public string pathToWinCutscene = "";
	public string pathToLoseCutscene = "";

	[Header("UI Elements")]
	public Button diceButton;
	public GameObject winPanel;
	public GameObject failPanel;
	public CanvasGroup fadeScreen;
    public CanvasGroup brokenGlass;

	public ColorWheelController colorWheel;

	public float fadeOutTime = 0.1f;
	public float fadeInTime = 0.1f;

	private ScreenFadeCallBack fadeInCallback; 
	private ScreenFadeCallBack fadeOutCallback;

    public GameController gameController;
    public AudioClip buttonClickAudioClip;

    public Button backButton;

	/// <summary>
	/// Show the window cutscene.
	/// </summary>
	public void ShowWinCutscene()
	{
		if (System.IO.File.Exists(pathToWinCutscene)) 
		{
			//Handheld.PlayFullScreenMovie (pathToWinCutscene);
		} 
		else 
		{
			winPanel.SetActive(true);
		}
		
		Debug.Log("Win");
	}

	/// <summary>
	/// Show the lose cutscene.
	/// </summary>
	public void ShowLoseCutscene()
	{
		if (System.IO.File.Exists(pathToWinCutscene)) 
		{
			//Handheld.PlayFullScreenMovie(pathToLoseCutscene);
		}
		else 
		{
			failPanel.SetActive(true);
		}

		Debug.Log("Lose");
	}

	/// <summary>
	/// Hide the win/fail screen.
	/// </summary>
	public void HideFinishScreen()
	{
		winPanel.SetActive(false);
		failPanel.SetActive(false);
	}

	/// <summary>
	/// Enable the dice button.
	/// </summary>
	public void EnableDiceButton(bool status)
	{
		diceButton.interactable = status;
	}
    
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
			"onUpdate", "OnFade",
			"onUpdateTarget", gameObject
		));
	}

	public void FadeInFinished()
	{
        fadeScreen.blocksRaycasts = true;

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
			"onUpdate", "OnFade",
			"onUpdateTarget", gameObject
		));
	}

    public void ShowBrokenGlass()
    {
        brokenGlass.gameObject.SetActive(true);
    }

    public void FadeOutBrokenGlass(ScreenFadeCallBack callback=null)
    {
        fadeOutCallback = callback;

        iTween.ValueTo(brokenGlass.gameObject, iTween.Hash (
            "from", 1.0f,
            "to", 0.0f,
            "time", fadeOutTime,
            "easetype", "linear",
            "onComplete", "FadeOutFinished",
            "onCompleteTarget", gameObject,
            "onUpdate", "OnFadeBrokenGlass",
            "onUpdateTarget", gameObject
        ));
    }

    public void OnFadeBrokenGlass(float value)
    {
        brokenGlass.alpha = value;
    }

	public void FadeOutFinished()
	{
        fadeScreen.blocksRaycasts = false;
        brokenGlass.gameObject.SetActive(false);

		if(fadeOutCallback != null)
			fadeOutCallback();
	}

	public void OnFade(float value)
	{
		fadeScreen.alpha = value;
	}

	public void PressStart()
	{
		FadeOut();
	}

	public void HideColorWheel()
	{
		colorWheel.HideColorWheel();
	}

	public void ShowColorWheel()
	{
		colorWheel.ShowColorWheel();
	}

    public void BackToMenu()
    {
        gameController.BackToMenu();
        backButton.enabled = false;
    }

    public void PlayButtonClickSound()
    {
        AudioManager.Instance.SetSFXChannel(buttonClickAudioClip, null, 0, 1);
    }
}
