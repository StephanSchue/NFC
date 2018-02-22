using UnityEngine;
using System.Collections;

public class RandomSpriteChooser : MonoBehaviour 
{
	private SpriteRenderer spriteRenderer;
	public Sprite[] pool;
	public bool flipX = true;
	public bool ShuffleOnEnable = false;

	private void OnEnable()
	{
		Initialize();

		if(ShuffleOnEnable)
			Spawn();
	}

	public void Initialize()
	{
		if(spriteRenderer == null)
			spriteRenderer = GetComponent<SpriteRenderer>();
	}

	/// <summary>
	/// Spawn the a sprite of the given pool
	/// </summary>
	/// <param name="index">Index: -1 = Random / > -1 = specific index</param>
	public void Spawn(int index=-1)
	{
		if (index == -1) 
		{
			// Random
			spriteRenderer.sprite = pool[Random.Range(0, pool.Length)];
		} 
		else 
		{
			// Special Sprite
			spriteRenderer.sprite = pool[(index < pool.Length - 1 ? index : 0)];
		}

		if(flipX)
		{
			spriteRenderer.flipX = Random.Range(0, 2) == 1 ? true : false;
		} 
		else 
		{
			spriteRenderer.flipX = false;
		}
	}
}
