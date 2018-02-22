using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SwipeVisuale : MonoBehaviour 
{
	public InputManager inputManager;
	private Text text;

	private void Awake()
	{
		text = GetComponent<Text>();
	}

	/// <summary>
	/// Update method
	/// </summary>
	private void FixedUpdate() 
	{
		text.text = inputManager.IsSwipePossilbe() ? "Swipe On" : "Swipe Off";
	}
}
