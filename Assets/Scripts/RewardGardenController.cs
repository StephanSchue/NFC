using UnityEngine;
using System.Collections;

public class RewardGardenController : MonoBehaviour 
{
	public StartscreenController startScreenController;
	public PlaceSeedController placeSeedController;

    public Transform flowerContainer;

	public GameObject[] seedPlaces = new GameObject[50];
	public GameObject[] seedPrefabs;

    public float star_particle_duration;
    public float cloud_particle_duration;
    public ParticleSystem star_particle_system;
    public ParticleSystem cloud_particle_system;
    public AudioClip plant_seed_audio;

	private int index = 0;

	/// <summary>
	/// Start this instance.
	/// </summary>
	protected void Awake()
	{
		placeSeedController.OnPlaceASeed += PlaceASeed;
		startScreenController.OnLoadSaveGameComplete += DrawGarden;
		startScreenController.OnClearGameComplete += DrawGarden;

        seedPrefabs = Resources.LoadAll<GameObject>("Flowers");
	}

	protected void OnDestroy()
	{
		placeSeedController.OnPlaceASeed -= PlaceASeed;
		startScreenController.OnLoadSaveGameComplete -= DrawGarden;
		startScreenController.OnClearGameComplete -= DrawGarden;
	}

	/// <summary>
	/// Starts the place seed controlle.
	/// </summary>
	public void StartPlaceSeed()
	{
		System.Array types = System.Enum.GetValues(typeof(SeedType));;

		placeSeedController.SetSeed((SeedType)types.GetValue(Random.Range(0, types.Length)));
		placeSeedController.enabled = true;
	}

	/// <summary>
	/// Stops the place seed controller.
	/// </summary>
	public void StopPlaceSeed()
	{
		placeSeedController.enabled = false;
	}

	/// <summary>
	/// Places a new Seed.
	/// </summary>
	/// <param name="type">Type.</param>
	/// <param name="position">Position.</param>
	public void PlaceASeed(Vector3 position, SpriteRenderer parentSpriteRenderer)
	{
		// Disable Cursor
		StopPlaceSeed();

        index = startScreenController.GetCountPlacedSeeds();

        if(index >= seedPlaces.Length)
        {
            index = 0;
            Destroy(seedPlaces[index].gameObject);
        }

        int seedIndex = Random.Range(0, seedPrefabs.Length);

        seedPlaces[index] = GameObject.Instantiate(seedPrefabs[seedIndex], position, Quaternion.identity) as GameObject; //new GameObject(string.Format("{0}_{1}", index, type));
        seedPlaces[index].transform.parent = flowerContainer;
        seedPlaces[index].transform.localScale = seedPrefabs[seedIndex].transform.localScale;

		RandomTextureChooser randomTextureChooser = seedPlaces[index].GetComponent<RandomTextureChooser>();

		if(randomTextureChooser != null) 
		{
			randomTextureChooser.Spawn(-1);
		}

        ParticleSystem partSys = seedPlaces[index].GetComponentInChildren<ParticleSystem>();

        if(partSys)
            partSys.transform.localScale *= 5.0f;

		SpriteRenderer newSpriteRenderer = seedPlaces[index].GetComponent<SpriteRenderer>();
		newSpriteRenderer.sortingLayerName = parentSpriteRenderer.sortingLayerName;
		newSpriteRenderer.sortingOrder = -1;

        startScreenController.AddNewPlant(seedIndex, seedPlaces[index].transform.position, newSpriteRenderer);
        PlayPlantingMagic(seedPlaces[index].transform.position, parentSpriteRenderer.sortingLayerName);
		++index;
	}

	/// <summary>
    /// Place a given Seed (existed)
	/// </summary>
	/// <returns>The plant.</returns>
	/// <param name="type">Type.</param>
	/// <param name="position">Position.</param>
	/// <param name="spriteName">Sprite name.</param>
	/// <param name="sortingLayer">Sorting layer.</param>
	/// <param name="flipX">If set to <c>true</c> flip x.</param>
    private SpriteRenderer CreatePlant(int seedIndex, Vector3 position, string spriteName, string sortingLayer, bool flipX)
	{
		// Place Flower at point & play animation
        seedPlaces[index] = GameObject.Instantiate(seedPrefabs[seedIndex], position, Quaternion.identity) as GameObject; //new GameObject(string.Format("{0}_{1}", index, type));
        seedPlaces[index].transform.parent = flowerContainer;
        seedPlaces[index].transform.position = position;
        seedPlaces[index].transform.localScale = seedPrefabs[seedIndex].transform.localScale;

        ParticleSystem partSys = seedPlaces[index].GetComponentInChildren<ParticleSystem>();

        if(partSys)
            partSys.transform.localScale *= 5.0f;

		SpriteRenderer newSpriteRenderer = seedPlaces[index].GetComponent<SpriteRenderer>();

		if(newSpriteRenderer)
		{
			newSpriteRenderer.sortingLayerName = sortingLayer;
			newSpriteRenderer.sortingOrder = -1;
			//newSpriteRenderer.sprite = GetSeedSpriteByName(spriteName);
			newSpriteRenderer.flipX = flipX;
		}

        //PlayPlantingMagic(seedPlaces[index].transform.position, sortingLayer);
		++index;
        
		return newSpriteRenderer;
	}

    public void PlayPlantingMagic(Vector3 position, string sortingLayer)
    {
        AudioManager.Instance.SetSFXChannel(plant_seed_audio, null, 0, 2);

        // Stars
        ParticleSystem partSys = ParticleSystem.Instantiate(star_particle_system, position, Quaternion.identity) as ParticleSystem;
        ParticleSystemRenderer partSysRenderer = partSys.GetComponent<Renderer>() as ParticleSystemRenderer;
       
        partSysRenderer.sortingLayerName = sortingLayer;

        StartCoroutine(ParticleEffectRunning(partSys, star_particle_duration));
        Destroy(partSys.gameObject, 10.0f);

        // Clouds
        ParticleSystem partSys2 = ParticleSystem.Instantiate(cloud_particle_system, position, Quaternion.identity) as ParticleSystem;
        ParticleSystemRenderer partSysRenderer2 = partSys2.GetComponent<Renderer>() as ParticleSystemRenderer;

        partSysRenderer2.sortingLayerName = sortingLayer;

        StartCoroutine(ParticleEffectRunning(partSys2, cloud_particle_duration));
        Destroy(partSys2.gameObject, 10.0f);
    }

    private IEnumerator ParticleEffectRunning(ParticleSystem particleSystem, float duration)
    {
        particleSystem.Play();

        yield return new WaitForSeconds(duration);

        particleSystem.Stop();
    }


	public Sprite GetSeedSpriteByName(string needle)
	{
		RandomTextureChooser randomTextureChooser = null;

		for(int i = 0; i < seedPrefabs.Length; i++) 
		{
			randomTextureChooser = seedPrefabs[i].GetComponent<RandomTextureChooser>();

			if(randomTextureChooser != null) 
			{
				for (int y = 0; y < randomTextureChooser.spritePool.Length; y++) 
				{
					if(randomTextureChooser.spritePool[y].name == needle) 
					{
						return randomTextureChooser.spritePool[y];
					}
				}
			}
		}

		return null;
	}

	/// <summary>
	/// Draws the saved garden.
	/// </summary>
	public void DrawGarden()
	{
		index = 0;

		// Draw the Seeds
		PlayerSaveGame.SeedPlace[] objects = startScreenController.LoadRewardGarden();

		if(objects.Length > 0) 
		{
			for (int i = 0; i < objects.Length; i++) 
			{
				if (objects[i].placed) 
				{
					CreatePlant(objects[i].type, objects [i].position, objects [i].sprite, objects [i].sortingLayer, objects [i].flipX);
				} 
				else 
				{
					if(seedPlaces[i] != null)
						Destroy(seedPlaces[i].gameObject);
				}
			}
		} 
		else 
		{
			for (int i = 0; i < seedPlaces.Length; i++) 
			{
				if(seedPlaces[i] != null)
					Destroy(seedPlaces[i].gameObject);
			}
		}
	}
}

public enum SeedType
{
	flower_daisies,
	flower_dandelion,
	flower_poppy,
	flower_sunflower,
    flower_cornflower,
    flower_gerbera_yellow,
    flower_gerbera_pink,
    flower_gerbera_orange
}