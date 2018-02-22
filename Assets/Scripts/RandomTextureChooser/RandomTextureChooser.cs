using UnityEngine;
using System.Collections;

/// <summary>
/// Component for Randomizing Textures depending on added renderer and choosen settings
/// </summary>
public class RandomTextureChooser : MonoBehaviour 
{
	public bool useSpriteRenderer { get; private set; }
	public bool rendererFound { get; private set; }

	// --- Sprite Specific ---
	[Header("Sprite Settings")]
	public SpriteRenderer spriteRenderer;
	public Sprite[] spritePool;

	// --- MeshRenderer Specific ---
	[Header("Mesh Settings")]
	public Renderer renderer;
	public Texture2D[] meshPool;
	public string textureLabel = "_MainTex";

	// Settings
	[Header("General Settings")]
	public bool randomFlipX = true;
	public bool randomFlipY = false;
	public bool shuffleOnEnable = false;

	private bool dontUseLastIndex = false;
	private int lastIndex = -1;

	public int poolSize 
	{
		get 
		{
			return useSpriteRenderer ? (spritePool != null ? spritePool.Length : 0) : (meshPool != null ? meshPool.Length : 0);
		}

		private set 
		{

		}
	}

	public Sprite currentSprite 
	{
		get {
			return spriteRenderer != null ? spriteRenderer.sprite : null;
		}

		private set 
		{

		}
	}

	public Texture2D currentTexture
	{
		get {
			return renderer != null ? (renderer.material.mainTexture as Texture2D) : null;
		}

		private set 
		{

		}
	}

	public bool currentFlipX
	{
		get 
		{
			return useSpriteRenderer ? (spriteRenderer.flipX) : (renderer.material.mainTextureScale.x > 0 ? false : true);
		}

		private set 
		{

		}
	}

	public bool currentFlipY
	{
		get 
		{
			return useSpriteRenderer ? (spriteRenderer.flipY) : (renderer.material.mainTextureScale.y > 0 ? false : true);
		}

		private set 
		{

		}
	}


	#region Initalize

	/// <summary>
	/// Raises the enable event.
	/// </summary>
	private void OnEnable()
	{
		Initialize();

		if(shuffleOnEnable)
			Spawn();
	}

	/// <summary>
	/// Initialize this instance.
	/// </summary>
	public void Initialize()
	{
		// --- Register components ---
		rendererFound = false;

		if (renderer == null) 
		{
			renderer = GetComponent<Renderer>();
		} 
	
		if(spriteRenderer == null)
		{
			spriteRenderer = GetComponent<SpriteRenderer>();

		}

		rendererFound = renderer != null ? true : false;

		if(!rendererFound)
			rendererFound = spriteRenderer != null ? true : false;

		// --- Set Mode ---
		if(spriteRenderer != null) 
		{
			useSpriteRenderer = true;
		} 
		else 
		{
			useSpriteRenderer = false;
		}

		lastIndex = -1;
	}

	#endregion

	#region General Functions

	/// <summary>
	/// Spawn the a sprite of the given pool
	/// </summary>
	/// <param name="index">Index: -1 = Random / > -1 = specific index</param>
	public void Spawn(int index=-1)
	{
		SetTexture(index);
		FlipTexture(randomFlipX, randomFlipY);
	}

	/// <summary>
	/// Sets the texture.
	/// </summary>
	/// <param name="index">Index.</param>
	public void SetTexture(int index)
	{
		int newIndex = index;

		if(useSpriteRenderer) 
		{
			if(spritePool == null || spritePool.Length <= 0) 
			{
				Debug.Log("Texture pool is empty. Please setup your pool.");
				return;
			}

			// --- SpriteRenderer ---
			if (index == -1) 
			{
				// -- Random --
				newIndex = Random.Range(0, spritePool.Length);

				if (spritePool.Length > 1) 
				{
					while (newIndex == lastIndex) 
					{
						newIndex = Random.Range(0, spritePool.Length);
					}
				}

				spriteRenderer.sprite = spritePool[newIndex];
			} 
			else 
			{
				// -- Special Sprite --
				spriteRenderer.sprite = spritePool[(index < spritePool.Length - 1 ? index : 0)];
			}
		} 
		else 
		{
			if(meshPool == null || meshPool.Length <= 0) 
			{
				Debug.Log("Texture pool is empty. Please setup your pool.");
				return;
			}

			// --- MeshRenderer ---
			if (index == -1) 
			{
				// -- Random --
				newIndex = Random.Range(0, meshPool.Length);

				if (meshPool.Length > 1) 
				{
					while (newIndex == lastIndex) 
					{
						newIndex = Random.Range(0, meshPool.Length);
					}
				}

				try 
				{
					renderer.material.SetTexture(textureLabel, meshPool[newIndex]);
				} 
				catch (System.Exception ex) 
				{
					// Hide UnityEditor Notification, drawn as error
				}

			} 
			else 
			{
				// -- Special Texture --
				renderer.material.SetTexture(textureLabel, meshPool[(index < meshPool.Length - 1 ? index : 0)]);
			}
		}

		lastIndex = newIndex;
	}

	/// <summary>
	/// Flips the texture.
	/// </summary>
	/// <param name="flipX">If set to <c>true</c> flip x.</param>
	/// <param name="flipY">If set to <c>true</c> flip y.</param>
	public void FlipTexture(bool flipX, bool flipY)
	{
		if(useSpriteRenderer) 
		{
			// --- SpriteRenderer ---
			spriteRenderer.flipX = flipX ? (Random.Range(0, 2) == 1 ? true : false) : false;
			spriteRenderer.flipY = flipY ? (Random.Range(0, 2) == 1 ? true : false) : false;
		} 
		else 
		{
			// --- MeshRenderer ---
			Vector2 tilling = new Vector2(renderer.material.mainTextureScale.x, renderer.material.mainTextureScale.y);

			tilling.x = flipX ? (Random.Range(0, 2) == 1 ? -tilling.x : tilling.x) : tilling.x;
			tilling.y = flipY ? (Random.Range(0, 2) == 1 ? -tilling.y : tilling.y) : tilling.y;

			renderer.material.mainTextureScale = tilling;
		}
	}

	/// <summary>
	/// Sets the pool.
	/// </summary>
	/// <param name="sprites">Sprites.</param>
	public void SetPool(Sprite[] sprites)
	{
		spritePool = sprites;
	}

	/// <summary>
	/// Sets the pool.
	/// </summary>
	/// <param name="textures">Textures.</param>
	public void SetPool(Texture2D[] textures)
	{
		meshPool = textures;
	}

	#endregion
}
