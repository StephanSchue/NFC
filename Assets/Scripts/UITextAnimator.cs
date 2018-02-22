using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UITextAnimator : MonoBehaviour 
{
	#region Variables
	private Text component;

	[Header("Setup")]
	public string content;

	[Header("Additional Settings")]
	public bool playOnEnable = true;
	public bool loop = true;
	public bool pingpong = false;
	public float duration = 0.1f;

	public bool blinkingCursor = false;
	public string blinkingCursorChar = "";

	// Runtime variables
	private bool directionForward = true;
	private int charIndex = 0;
	private int maxCharIndex = 0;
	private bool blinkingCursorCharToogle = true;

	private float nextCharPlacementTime = 0.0f;

	private bool animation = true;

	#endregion

	#region Functions

	/// <summary>
	/// Raises the enable event.
	/// </summary>
	private void OnEnable()
	{
		component = GetComponent<Text>();

		if (playOnEnable) 
		{
			StartAnimation();
		}
	}

	/// <summary>
	/// Starts the animation.
	/// </summary>
	public void StartAnimation()
	{
		charIndex = 0;
		maxCharIndex = content.Length;

		nextCharPlacementTime = 0.0f;
		animation = directionForward = true;
	}

	/// <summary>
	/// Stops the animation.
	/// </summary>
	public void StopAnimation()
	{
		animation = false;
	}

	/// <summary>
	/// Run the Animation-cylce if its started
	/// </summary>
	public void FixedUpdate()
	{
		if(!animation)
			return;

		if(Time.time > nextCharPlacementTime) 
		{
			if (charIndex > maxCharIndex) 
			{
				if(pingpong) 
				{
					directionForward = !directionForward;
					charIndex = maxCharIndex;
				} 
				else if (loop) 
				{
					charIndex = 0;
				} 
				else 
				{
					StopAnimation();
					return;
				}
			} 
			else if(pingpong && charIndex == -1) 
			{
				directionForward = !directionForward;
				charIndex = 0;
			}

			component.text = content.Substring(0, directionForward ? charIndex++ : charIndex--);

			if(blinkingCursor) 
			{
				component.text += blinkingCursorCharToogle ? blinkingCursorChar : "";
				blinkingCursorCharToogle = !blinkingCursorCharToogle;
			}

			nextCharPlacementTime = Time.time + duration;
		}
	}

	#endregion
}
