using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class FruitToColor : Singleton<FruitToColor> 
{
	protected FruitToColor() {} // guarantee this will be always a singleton only - can't use the constructor!

	[SerializeField][HideInInspector]
	public List<FruitEntry> fruitEntries = new List<FruitEntry>();

	/// <summary>
	/// Adds the node.
	/// </summary>
	/// <param name="fruitType">Fruit type.</param>
	/// <param name="fruitColor">Fruit color.</param>
	public void AddNode(string fruitType, FruitColor fruitColor)
	{
		fruitEntries.Add(new FruitEntry(fruitType, fruitColor));
	}

	/// <summary>
	/// Removes the node.
	/// </summary>
	/// <param name="entry">Entry.</param>
	public void RemoveNode(FruitEntry entry)
	{
		fruitEntries.Remove(entry);
	}

	/// <summary>
	/// Gets the color of fruit.
	/// </summary>
	/// <returns>The color of fruit.</returns>
	/// <param name="fruitType">Fruit type.</param>
	public FruitColor GetColorOfFruit(string fruitType)
	{
		for(int i = 0; i < fruitEntries.Count; i++) 
		{
			if(fruitType == fruitEntries[i].FruitType) 
			{
				return fruitEntries[i].FruitColor;
			}
		}

		return FruitColor.None;
	}

	/// <summary>
	/// Gets the color of the fruits for.
	/// </summary>
	/// <returns>The fruits for color.</returns>
	/// <param name="fruitColor">Fruit color.</param>
	public string[] GetFruitsForColor(FruitColor fruitColor)
	{
		List<string> fruits = new List<string>();

		for(int i = 0; i < fruitEntries.Count; i++) 
		{
			if(fruitColor == fruitEntries[i].FruitColor) 
			{
				fruits.Add(fruitEntries[i].FruitType);
			}
		}

		return fruits.ToArray();
	}

	/// <summary>
	/// Clear this instance.
	/// </summary>
	public void Clear()
	{
		fruitEntries.Clear();
	}
}

[System.Serializable]
public struct FruitEntry
{
	[SerializeField]
	public string FruitType;
	[SerializeField]
	public FruitColor FruitColor;

	public FruitEntry(string fruitType, FruitColor fruitColor)
	{
		FruitType = fruitType;
		FruitColor = fruitColor;
	}
}