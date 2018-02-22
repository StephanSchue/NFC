using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(RandomTextureController))]
public class RandomSpriteControllerEditor : Editor 
{
	private RandomTextureController myController = null;

	private void OnEnable()
	{
		myController = (RandomTextureController)target;
		myController.GetChildren();
	}

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		if(GUILayout.Button("Generate"))
		{
			myController.Generate();
		}

		Undo.RecordObject(target, "Generate Sprites");
	}
}
