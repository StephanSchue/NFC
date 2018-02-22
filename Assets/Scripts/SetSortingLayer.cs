using UnityEngine;
using System.Collections;

public class SetSortingLayer : MonoBehaviour 
{
	public string sortinglayer;

	// Use this for initialization
	public void SetSorting() 
	{
		foreach (SpriteRenderer spriteRenderer in GetComponentsInChildren<SpriteRenderer>()) 
		{
			spriteRenderer.sortingLayerName = sortinglayer;
		}
	}

}
