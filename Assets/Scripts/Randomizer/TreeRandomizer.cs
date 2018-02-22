#if UNITY_EDITOR

using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

[ExecuteInEditMode]
public class TreeRandomizer : MonoBehaviour 
{
	[Header("Resources")]
	public string TreeTopResourcesPath = "TreeTop";
	public string TreeBarkResourcesPath = "TreeBark";
	public string LeafsResourcesPath = "Leafs";

	[HideInInspector()]
	public GameObject[] TreeBarks;
	[HideInInspector()]
	public GameObject[] TreeTops;
	[HideInInspector()]
	public GameObject[] Leafs;

	[Header("Custom Anchor")]
	public bool UseBark = true;
	public GameObject CustomTreeTop;
	public GameObject CustomTreeBark;

	private GameObject curObject;

	private List<Transform> treeLeafAnchorPoints;
	private List<Transform> treeFruitAnchorPoints;
	private List<SpriteRenderer> fruitObjects;
	public bool VisualizingFruits = false;

	[Header("Filesave settings")]
	public string SavePath = "Alpha/RandomizerExport/";
	public bool OverrideBySaving = false;

	[HideInInspector()]
	public bool objectSaved = false;

	/// <summary>
	/// Use this for initialization
	/// </summary>
	private void Update() 
	{
		TreeTops = Resources.LoadAll<GameObject>(TreeTopResourcesPath);
		TreeBarks = Resources.LoadAll<GameObject>(TreeBarkResourcesPath);
		Leafs = Resources.LoadAll<GameObject>(LeafsResourcesPath);
	}

	/// <summary>
	/// Generates the object.
	/// </summary>
	/// <param name="bark">Bark.</param>
	/// <param name="treetop">Treetop.</param>
	public void GenerateObject(GameObject bark=null, GameObject treetop=null)
	{
		if (curObject)
			DestroyImmediate(curObject);

		string barkName = "";
		string treeTopName = "";

		// --- Prepair ---

		// -- Bark --
		if (bark != null) 
		{
			barkName = bark.name;
		}
		else if(UseBark)
		{
			int treeBarkNumber = Random.Range (0, TreeBarks.Length);
			bark = TreeBarks[treeBarkNumber];
			barkName = TreeBarks [treeBarkNumber].name;
		}

		// -- Treetop --
		if (treetop != null) 
		{
			treeTopName = treetop.name;
		}
		else
		{
			int treeTopNumber = Random.Range (0, TreeTops.Length);
			treetop = TreeTops[treeTopNumber];
			treeTopName = TreeTops[treeTopNumber].name;
		}

		// -- Leafs --
		Transform[] _transformTreeChilds = treetop.GetComponentsInChildren<Transform>();
		Transform[] _transformBarkChilds = bark.GetComponentsInChildren<Transform>();

		// Comnbine paceholders on bark and on treetop
		ArrayUtility.AddRange<Transform>(ref _transformTreeChilds, _transformBarkChilds);

		Transform[] _transformChilds = _transformTreeChilds;

		treeLeafAnchorPoints = new List<Transform>();
		treeFruitAnchorPoints = new List<Transform>();
		fruitObjects = new List<SpriteRenderer>();

		if (treeLeafAnchorPoints != null) 
		{
			for (int i = 0; i < _transformChilds.Length; i++) 
			{
				if (_transformChilds[i].CompareTag("TreeLeafAnchor")) 
				{
					treeLeafAnchorPoints.Add(_transformChilds[i]);
				}
				else if (_transformChilds[i].CompareTag("TreeFruitAnchor")) 
				{
					treeFruitAnchorPoints.Add(_transformChilds[i]);
				}
			}
		}
			
		// --- Create Object ---
		curObject = new GameObject();
		objectSaved = false;
		Vector3 treeTopAnchorPosition = Vector3.zero;

		// -- Bark --
		if (UseBark) 
		{
			GameObject barkObject = Instantiate<GameObject> (bark);
			barkObject.transform.parent = curObject.transform;
		
			// - Read AchnorPoint of the Bark -
			Transform[] treeTopAnchor = barkObject.GetComponentsInChildren<Transform>();

			if (treeTopAnchor != null) 
			{
				for (int i = 0; i < treeTopAnchor.Length; i++) 
				{
					if (treeTopAnchor[i].CompareTag("TreeTopAnchor")) 
					{
						treeTopAnchorPosition = treeTopAnchor[i].position;
						break;
					}
				}
			} 
		}

		// -- TreeTop --
		GameObject topObject = Instantiate<GameObject>(treetop);
		topObject.transform.parent = curObject.transform;
		topObject.transform.position = treeTopAnchorPosition;

		// -- Leafs --
		if(treeLeafAnchorPoints.Count > 0)
		{
			GameObject leafObject = null;
			SpriteRenderer anchorSpriteRenderer = null;
			SpriteRenderer prefabSpriteRenderer = null;

			for (int i = 0; i < treeLeafAnchorPoints.Count; i++) 
			{
				if (treeLeafAnchorPoints[i].CompareTag("TreeLeafAnchor")) 
				{
					leafObject = Leafs[Random.Range (0, Leafs.Length)];
					anchorSpriteRenderer = treeLeafAnchorPoints[i].GetComponent<SpriteRenderer>();
					prefabSpriteRenderer = leafObject.GetComponent<SpriteRenderer>();

					anchorSpriteRenderer.sprite = prefabSpriteRenderer.sprite;
					anchorSpriteRenderer.enabled = true;
				}
			}
		}

		// -- Collect fruits for visualisation --
		fruitObjects.Clear();

		if (treeFruitAnchorPoints != null) 
		{
			SpriteRenderer[] spriteChildObjects = curObject.GetComponentsInChildren<SpriteRenderer>(true);

			for (int i = 0; i < spriteChildObjects.Length; i++) 
			{
				if (spriteChildObjects[i].CompareTag("TreeFruitAnchor")) 
				{
					fruitObjects.Add(spriteChildObjects[i]);
				}
			}

			VisualizeFruits(VisualizingFruits);
		}

		// -- Set Name --
		curObject.name = string.Concat(barkName, treeTopName);

		// -- Adding Additional Components --
		curObject.AddComponent<TreeController>();

	}

	/// <summary>
	/// Generates all combinations.
	/// </summary>
	public void GenerateAllCombinations()
	{
		if(CustomTreeBark != null && CustomTreeTop != null)
		{
			// --- Custom One ---
			GenerateObject(CustomTreeBark, CustomTreeTop);
			SaveObject();
		}
		else if(CustomTreeBark != null)
		{
			// --- Custom TreeTop ---
			for (int i = 0; i < TreeTops.Length; i++) 
			{
				GenerateObject(CustomTreeBark, TreeTops[i]);
				SaveObject();
			}
		}
		else if(CustomTreeTop != null)
		{
			// --- Custom TreeTop ---
			for (int i = 0; i < TreeBarks.Length; i++) 
			{
				GenerateObject(TreeBarks[i], CustomTreeTop);
				SaveObject();
			}
		}
		else
		{
			// --- Fully Random ---
			for (int i = 0; i < TreeBarks.Length; i++) 
			{
				for (int y = 0; y < TreeTops.Length; y++) 
				{
					GenerateObject(TreeBarks[i], TreeTops[y]);
					SaveObject();
				}
			}
		}
	}

	/// <summary>
	/// Saves the object.
	/// </summary>
	public void SaveObject()
	{
		if(curObject == null)
			return;

		if (!CheckCurrentObjectExists () || OverrideBySaving) 
		{
			PrefabUtility.CreatePrefab(string.Concat(SavePath, curObject.name, ".prefab"), curObject);
			objectSaved = true;
		}
	}

	/// <summary>
	/// Checks the current object exists.
	/// </summary>
	public bool CheckCurrentObjectExists()
	{
		if (curObject != null) 
		{
			return AssetDatabase.FindAssets(curObject.name).Length > 0 ? true : false;
		} 
		else 
		{
			return false;
		}
	}

	/// <summary>
	/// Visualize the fruits.
	/// </summary>
	public void VisualizeFruits(bool visualize)
	{
		if (fruitObjects.Count > 0) 
		{
			for (int i = 0; i < fruitObjects.Count; i++) 
			{
				fruitObjects[i].enabled = visualize;
			}
		}
	}

	/// <summary>
	/// Toogles the visualize fruits.
	/// </summary>
	public void ToogleVisualizeFruits()
	{
		VisualizingFruits = !VisualizingFruits;
		VisualizeFruits(VisualizingFruits);
	}
}

#endif