using UnityEngine;
using System.Collections;

public class BubbleDrawerInputController : MonoBehaviour 
{
	private BubbleDrawer bubbleDrawer;

	// Use this for initialization
	private void Start() 
	{
		bubbleDrawer = GetComponent<BubbleDrawer>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetMouseButton(0))
		{
			bubbleDrawer.DrawStencial();
		}
	}
}
