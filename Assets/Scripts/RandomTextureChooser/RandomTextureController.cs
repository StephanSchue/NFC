using UnityEngine;
using System.Collections;

/// <summary>
/// Helper-class for randomize multible childs with "RandomTextureChooser" with one click.
/// </summary>
public class RandomTextureController : MonoBehaviour 
{
	private RandomTextureChooser[] chooser;
	public bool autoRandomize = true;

	// Use this for initialization
	private void OnEnable() 
	{
		GetChildren();

		if(autoRandomize)
			Generate();
	}

	public void GetChildren()
	{
		chooser = GetComponentsInChildren<RandomTextureChooser>();

		if(chooser != null && chooser.Length > 0) 
		{
			for (int i = 0; i < chooser.Length; i++) 
			{
				chooser[i].Initialize();
			}
		}
	}

	public void Generate()
	{
		if(chooser != null && chooser.Length > 0)
		{
			for(int i = 0; i < chooser.Length; i++) 
			{
				if(chooser[i].enabled)
					chooser[i].Spawn();
			}
		}
	}
}
