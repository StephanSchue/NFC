using UnityEngine;
using System.Collections;

/// <summary>
/// Component for Randomizing Prefab
/// </summary>
public class RandomPrefabChooser : MonoBehaviour 
{
	// --- Sprite Specific ---
    public GameObject prefab { get; private set; }
    public GameObject[] prefabPool;

    public string sortingLayerName;

	// Settings
	[Header("General Settings")]
	public bool shuffleOnEnable = false;

	private bool dontUseLastIndex = false;
	private int lastIndex = -1;

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
        SetPrefab(index);
	}

	/// <summary>
	/// Sets the texture.
	/// </summary>
	/// <param name="index">Index.</param>
	public void SetPrefab(int index)
	{     
        if(prefabPool != null && prefabPool.Length > 0)
        {
            int newIndex = index == -1 ? Random.Range(0, prefabPool.Length) : index;

            prefab = GameObject.Instantiate(prefabPool[newIndex]);
            prefab.transform.parent = transform;
            prefab.transform.localPosition = Vector3.zero;
            prefab.transform.localRotation = Quaternion.identity;
            prefab.transform.localScale = new Vector3(prefabPool[newIndex].transform.localScale.x, prefabPool[newIndex].transform.localScale.y, prefabPool[newIndex].transform.localScale.z) * 1.4f;
            prefab.transform.localScale = new Vector3(Mathf.Clamp(prefab.transform.localScale.x, 1.0f, 2.0f) * (Random.Range(0, 2) == 1 ? 1 : -1), Mathf.Clamp(prefab.transform.localScale.y, 1.0f, 2.0f), Mathf.Clamp(prefab.transform.localScale.z, 1.0f, 2.0f));

            SpriteRenderer renderer = prefab.GetComponent<SpriteRenderer>();

            if(renderer != null)
            {
                renderer.sortingLayerName = sortingLayerName;
            }

            lastIndex = newIndex; 
        }
	}


	#endregion
}
