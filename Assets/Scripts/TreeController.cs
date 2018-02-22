using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/*
public enum FruitType
{
	None,
	GreenApple,
	OrangePears,
	RedApple,
	RedCherries,
	VioletPlum,
	YellowApple,
	YellowLemons,
	YellowPears
}
*/

public class TreeController : MonoBehaviour 
{
	public string FruitSourceRootFolder = "Fruits";

	public string[] possibleFruits;

	[HideInInspector()]
	public List<GameObject> fruits = new List<GameObject>();

	[HideInInspector()]
	public List<Transform> FruitPlaceholders = new List<Transform>();

	private void Start()
	{
		GetFruitPlaceholders();

		if(transform.parent.GetComponent<SetSortingLayer>())
		{
			transform.parent.GetComponent<SetSortingLayer>().SetSorting();
		}
	}

    public Transform TreeTop = null;

	#region Fruits

    protected void Awake()
    {
        Transform child = null;

        for (int i = 0; i < transform.childCount; i++)
        {
            child = transform.GetChild(i);

            if(child != null && child.CompareTag("TreeTop"))
            {
                TreeTop = child;
                break;
            }
        }
    }

	/// <summary>
	/// Creates the fruits on a tree.
	/// </summary>
	public void CreateTreeFruits()
	{
		GetFruitPlaceholders();

		if(FruitPlaceholders.Count <= 0)
			return;

		if(possibleFruits.Length > 0)
		{
			// Manually posibility
			string randomFruit = possibleFruits[UnityEngine.Random.Range(0, possibleFruits.Length)];
			SetupTreeFruits(randomFruit);
		}
		else
		{
			// Fully random
			SetupTreeFruits(GetRandomFruitType());
		}
	}

	/// <summary>
	/// Creates the tree fruits by diceColor.
	/// </summary>
	/// <param name="diceColor">Dice color.</param>
	public void CreateTreeFruits(FruitColor color)
	{
		GetFruitPlaceholders();

		if(FruitPlaceholders.Count <= 0)
			return;

		SetupTreeFruits(GetFruitTypeByColor(color));
	}

    /// <summary>
    /// Creates the tree fruits by diceColor.
    /// </summary>
    /// <param name="diceColor">Dice color.</param>
    public void CreateTreeFruits(string fruitName)
    {
        GetFruitPlaceholders();

        if(FruitPlaceholders.Count <= 0)
            return;

        SetupTreeFruits(fruitName);
    }
		
	/// <summary>
	/// Setups the tree fruits.
	/// </summary>
	/// <param name="fruitType">Fruit type.</param>
	private void SetupTreeFruits(string fruitType)
	{
		if(FruitPlaceholders.Count <= 0)
			return;

		// .. Clear old stuff --
		ClearAllFruits();

		// -- Collect Variations --
		GameObject[] fuitVariationPrefabs = GetFuitsFromResources(fruitType);

		// -- Place Variations --
		if(fuitVariationPrefabs != null && fuitVariationPrefabs.Length > 0)
		{
			GameObject newFruit = null;
			SpriteRenderer newFruitSpriteRenderer = null;

			//fuitVariationPrefabs.

			GameObject[] randomVariationList = Shuffle<GameObject>(fuitVariationPrefabs);
			GameObject randomFruit = null;
			int variationIndicatior = 0;

			// -- Instanciate Fruits --
			for (int i = 0; i < FruitPlaceholders.Count; i++) 
			{
				// - Random with shuffled list -> minimal duplicates -
				randomFruit = randomVariationList[variationIndicatior];

				// if there are more anchor-points than variations, reuse variations from the begin of the list
				if (i >= randomVariationList.Length-1) 
				{
					variationIndicatior = 0;
				} 
				else 
				{
					++variationIndicatior;
				}
				
				// - Random with duplicates -
				// fuitVariationPrefabs[UnityEngine.Random.Range (0, fuitVariationPrefabs.Length)];

				newFruit = Instantiate<GameObject>(randomFruit);
				newFruit.transform.parent = FruitPlaceholders[i].transform;
				newFruit.transform.localPosition = Vector2.zero;
                //newFruit.transform.localRotation = Quaternion.identity;

				// - Randomize the x flip - // Scale Flip is a better aproach for collider adjustment
				//newFruitSpriteRenderer = newFruit.GetComponent<SpriteRenderer>();
				//newFruitSpriteRenderer = Convert.ToBoolean(UnityEngine.Random.Range(0, 2));
				newFruit.transform.localScale = new Vector3(UnityEngine.Random.Range(0, 2) == 1 ? -1f : 1f, 1f, 1f);

				// - Add Components -
				//newFruit.AddComponent<FruitController>();

				// - Add to list -
				fruits.Add(newFruit);
			}
		}
	}

	/// <summary>
	/// Clears all fruits.
	/// </summary>
	public void ClearAllFruits()
	{
		if(fruits.Count <= 0)
			return;
		
		for (int i = 0; i < fruits.Count; i++) 
		{
			DestroyImmediate(fruits[i]);
		}

		fruits.Clear();
	}

	/// <summary>
	/// Gets the fruit placeholders.
	/// </summary>
	public void GetFruitPlaceholders()
	{
		Transform[] childObjects = GetComponentsInChildren<Transform>();
		FruitPlaceholders.Clear();

		for (int i = 0; i < childObjects.Length; i++) 
		{
			if(childObjects[i].CompareTag("TreeFruitAnchor")) 
			{
				FruitPlaceholders.Add(childObjects[i]);

				if(childObjects[i].childCount > 0)
				{
					fruits.Add(childObjects[i].GetChild(0).gameObject);
				}
			}
		}
	}
		
	/// <summary>
	/// Get a fruit type by diceColor.
	/// </summary>
	/// <returns>The fruit type by color.</returns>
	/// <param name="diceColor">Dice color.</param>
	public string GetFruitTypeByColor(FruitColor fruitColor)
	{
		if(possibleFruits != null && possibleFruits.Length > 0)
		{
			string[] shuffledPossibleFruits = Shuffle<string>(possibleFruits);
			string[] fruitTypes = FruitToColor.Instance.GetFruitsForColor(fruitColor);

			// -- If user set manually possible fruits --
			for (int i = 0; i < fruitTypes.Length; i++) 
			{
				for (int y = 0; i < possibleFruits.Length; y++) 
				{
					if (fruitTypes[i] == possibleFruits[i]) 
					{
						return fruitTypes[i];
					}
				}
			}
		}
		else
		{
			// -- If any fruit is possible --
			return GetRandomFruitType(fruitColor);
		}

		return null;
	}

	/// <summary>
	/// Gets the random entry of the fruit enum.
	/// </summary>
	private string GetRandomFruitType()
	{
		// Random Color
		Array values = Enum.GetValues(typeof(FruitColor));
		System.Random random = new System.Random();
		FruitColor randomFruitColor = (FruitColor)values.GetValue(random.Next(values.Length));

		return GetRandomFruitType(randomFruitColor);
	}

	/// <summary>
	/// Gets the random entry of the fruit enum.
	/// </summary>
	private string GetRandomFruitType(FruitColor fruitColor)
	{
		string[] fruittypes = FruitToColor.Instance.GetFruitsForColor(fruitColor);

		if(fruittypes == null || fruittypes.Length <= 0) 
		{
			Debug.Log("No Fruittype found!");
			return null;
		} 
		else 
		{
			return fruittypes [UnityEngine.Random.Range (0, fruittypes.Length)];
		}
	}
		
	/// <summary>
	/// Gets the fuits from resources.
	/// </summary>
	/// <returns>The fuits from resources.</returns>
	/// <param name="fruitType">Fruit type.</param>
	public GameObject[] GetFuitsFromResources(string fruitType)
	{
		return Resources.LoadAll<GameObject>(FruitSourceRootFolder + "/" + fruitType);
	}

	#endregion

	#region FruitToColor

	/// <summary>
	/// Compare the given color to the fruit types that defined on the tree.
	/// </summary>
	/// <returns><c>true</c> if the color is possible; otherwise, <c>false</c>.</returns>
	/// <param name="diceColor">Dice color.</param>
	public bool IsColorPossible(FruitColor fruitColor)
	{
		FruitColor colorOfTheFruit = FruitColor.None;

		if(possibleFruits != null && possibleFruits.Length > 0)
		{
			string[] fruitTypes = FruitToColor.Instance.GetFruitsForColor(fruitColor);

			// -- If user set manually possible fruits --
			for (int i = 0; i < fruitTypes.Length; i++) 
			{
				for (int y = 0; i < possibleFruits.Length; y++) 
				{
					if (fruitTypes[i] == possibleFruits[i]) 
					{
						return true;
					}
				}
			}
		}
		else
		{
			// -- If any fruit is possible --
			return true;
		}

		return false;
	}

    public FruitController GetRandomFruit()
    {
        FruitController fruitController = null;
        GameObject[] fruitsShuffles = Shuffle<GameObject>(fruits.ToArray());

        for (int i = 0; i < fruitsShuffles.Length; i++)
        {
            fruitController = fruitsShuffles[i].GetComponent<FruitController>();
                
            if(fruitController.inTheBasket)
            {
                fruitController = null;
            }
            else
            {
                break;
            }
        }

        if(fruitController != null)
        {
            Animator treeTopAnimator = TreeTop.GetComponent<Animator>();

            if(treeTopAnimator != null)
            {
                //Debug.Log("AFGJ");
                //treeTopAnimator.SetTrigger("Shake");
            }
        }

        return fruitController;
    }

    #endregion

    public void Shaking()
    {

    }

    #region hint

    public void HighlightFruits(bool status)
    {
        FruitController curFruit = null;

        for (int i = 0; i < fruits.Count; i++)
        {
            curFruit = fruits[i].GetComponent<FruitController>();

            if (status)
            {
                curFruit.HighlightFruit();
            }
            else
            {
                curFruit.UnHighlightFruit();
            }
        }
    }

	#endregion

	/// <summary>
	/// Shuffle the array.
	/// </summary>
	/// <typeparam name="T">Array element type.</typeparam>
	/// <param name="array">Array to shuffle.</param>
	public T[] Shuffle<T>(T[] array)
	{
		System.Random random = new System.Random();
		for (int i = array.Length; i > 1; i--)
		{
			// Pick random element to swap.
			int j = random.Next(i); // 0 <= j <= i-1
			// Swap.
			T tmp = array[j];
			array[j] = array[i - 1];
			array[i - 1] = tmp;
		}
		return array;
	}
}
