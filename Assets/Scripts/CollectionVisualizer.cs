using UnityEngine;
using System.Collections;

public class CollectionVisualizer : MonoBehaviour 
{
	public Collectable[] collectables;

	private void OnEnable()
	{
		
	}

	public void SetCollectableAnchors()
	{
		collectables = GetComponentsInChildren<Collectable>();
	}
}
