using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Linq;

[CustomEditor(typeof(RandomTextureChooser))]
[CanEditMultipleObjects()]
public class RandomTextureChooserEditor : Editor 
{
	private RandomTextureChooser myController = null;

	// --- Sprite Specific ---
	//private SerializedProperty p_spriteRenderer;
	private SerializedProperty p_spritePool;

	// --- MeshRenderer Specifc ---
	//private SerializedProperty p_meshRenderer;
	private SerializedProperty p_meshPool;
	private SerializedProperty p_textureLabel;

	// --- MeshRenderer Specifc ---
	private SerializedProperty p_randomFlipX;
	private SerializedProperty p_randomFlipY;
	private SerializedProperty p_shuffleOnEnable;

	public string AssetPath = "";

	/// <summary>
	/// Raises the enable event.
	/// </summary>
	private void OnEnable()
	{
		myController = (RandomTextureChooser)target;

		//p_spriteRenderer = serializedObject.FindProperty("spriteRenderer");
		p_spritePool = serializedObject.FindProperty("spritePool");

		//p_meshRenderer = serializedObject.FindProperty("meshRenderer");
		p_meshPool = serializedObject.FindProperty("meshPool");
		p_textureLabel = serializedObject.FindProperty("textureLabel");

		p_randomFlipX = serializedObject.FindProperty("randomFlipX");
		p_randomFlipY = serializedObject.FindProperty ("randomFlipY");
		p_shuffleOnEnable = serializedObject.FindProperty("shuffleOnEnable");

		myController.Initialize();
	}

	/// <summary>
	/// Raises the inspector GUI event.
	/// </summary>
	public override void OnInspectorGUI()
	{
		EditorGUILayout.Space();
		EditorGUILayout.Space();

		// Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
		serializedObject.Update();

		if(!myController.rendererFound) 
		{
			EditorGUILayout.LabelField("No Renderer found. Do you want to add one?");

			if(GUILayout.Button("Add SpriteRenderer"))
			{
				myController.gameObject.AddComponent<SpriteRenderer>();
			} 

			if(GUILayout.Button("Add MeshRenderer"))
			{
				GameObject dummyObj = GameObject.CreatePrimitive (PrimitiveType.Quad);

				myController.gameObject.AddComponent<MeshRenderer>();
				myController.gameObject.AddComponent<MeshFilter>();

				try 
				{
					myController.gameObject.GetComponent<MeshFilter> ().mesh = dummyObj.GetComponent<MeshFilter>().mesh;
				} 
				catch (System.Exception ex) 
				{
						
				}

				DestroyImmediate(dummyObj);
			}

			/*
			if(GUILayout.Button("Add SkinnedMeshRenderer"))
			{
				GameObject dummyObj = GameObject.CreatePrimitive(PrimitiveType.Quad);

				myController.gameObject.AddComponent<SkinnedMeshRenderer>();

				myController.gameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh = dummyObj.GetComponent<MeshFilter>().mesh;
				myController.gameObject.GetComponent<SkinnedMeshRenderer>().material = dummyObj.GetComponent<MeshRenderer>().sharedMaterial;
				DestroyImmediate(dummyObj);
			} 
			*/

			myController.Initialize();
		}
		else
		{
			if(myController.useSpriteRenderer) 
			{
				// --- SpriteRenderer Specific ---
				EditorGUILayout.PropertyField(p_spritePool, true);
			} 
			else 
			{
				// --- MeshRenderer Specific ---
				EditorGUILayout.PropertyField(p_meshPool, true);

				if (myController.renderer.sharedMaterial != null) 
				{
					Shader shader = myController.renderer.sharedMaterial.shader;
					string textureNames = ""; 

					for (int i = 0; i < ShaderUtil.GetPropertyCount (shader); i++) 
					{
						if (ShaderUtil.GetPropertyType (shader, i) == ShaderUtil.ShaderPropertyType.TexEnv) 
						{
							textureNames += ShaderUtil.GetPropertyName (shader, i).ToString () + System.Environment.NewLine;
						}
					}

					EditorGUILayout.LabelField ("Possible Texture From Shader:", EditorStyles.boldLabel);
					EditorGUILayout.TextArea (textureNames);
				}

				EditorGUILayout.PropertyField(p_textureLabel);
			}

			// --- General Settings ---
			EditorGUILayout.PropertyField(p_randomFlipX);
			EditorGUILayout.PropertyField(p_randomFlipY);
			EditorGUILayout.PropertyField(p_shuffleOnEnable);

			if(GUILayout.Button("Generate (" + (myController.poolSize * (p_randomFlipX.boolValue ? 2 : 1) * (p_randomFlipY.boolValue ? 2 : 1)) + " possible variations)"))
			{
				myController.Spawn();
			}

			/*
			AssetPath = EditorGUILayout.TextField(AssetPath);

			if(GUILayout.Button("Add textures from path"))
			{
				if(myController.useSpriteRenderer) 
				{
					AssetPath = "Assets/Alpha/Source/";
					string[] lookFor = new string[] {AssetPath};

					Debug.Log(AssetDatabase.FindAssets("t:texture2D", lookFor).Length);
					//myController.SetPool(AssetDatabase.LoadAllAssetsAtPath(AssetPath).OfType<Sprite>());
				} 
				else 
				{
					myController.SetPool(AssetDatabase.LoadAllAssetsAtPath(AssetPath) as Texture2D[]);
				}
			}
			*/

			if(myController.useSpriteRenderer) 
			{
				Undo.RecordObject(myController.spriteRenderer, "Change Texture of Object");
			} 
			else 
			{
				Undo.RecordObject(myController.renderer, "Change Texture of Object");
			}
		}

		// Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
		serializedObject.ApplyModifiedProperties();
	}


}
