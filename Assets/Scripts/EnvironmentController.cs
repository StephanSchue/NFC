using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;
using System.Collections.Generic;

public class EnvironmentController : MonoBehaviour
{
    [Header("Day/Night")]
    public bool forceNight = false;
    public bool daynight = true;
    private bool isNight = false;
    public string startOfNight = "19:00";
    public string endOfNight = "6:00";
    public GameObject sun;
    public GameObject moon;
    public SpriteRenderer sky;
    public Sprite daySky;
    public Sprite nightSky;

    public bool IsNight { get { return isNight; } }

    public Color nightTint = Color.blue;
    //public ColorCorrectionCurves colorCorrectionCurves;

    [Header("Mole")]
    public MoleController mMole;
    public float moleSpawnTimeSpan = 2.0f;
    private float moleNextSpawnTime = 0.0f;
    private bool mMoleWorking = false;

    public SpawnpointInGrass[] moleSpawnpoints;
    public bool moleSpawn = false;
    private bool moleActivated = false;
    private int lastMoleIndex = 0;

    [Header("Pollen")]
    public GameObject pollenPrefab;
    public int maxPolleCount = 10;
    private int pollenCount = 0;
    public float pollenSpawnTimeSpan = 2.0f;
    private float pollenNextSpawnTime = 0.0f;
    public bool pollenSpawn = false;
    private bool pollenActivated = false;
    public string[] spawnSortingLayers;

    [Header("Bee")]
    public GameObject beePrefab;
    public float beeSpawnTimeSpan = 2.0f;
    private float beeNextSpawnTime = 0.0f;
    public bool beeSpawn = false;
    private bool beeActivated = false;
    private int lastBeeIndex = 0;
    public SpawnpointInGrass[] beeSpawnpoints;

    [Header("EarthWorm")]
    public EarthWormController earthWorm;
    public bool eathWormSpawn = false;
    private bool eathWormActivated = false;
    public float earthSpawnTimeSpan = 2.0f;
    private float earthNextSpawnTime = 0.0f;
    private int lastFruitIndex = 0;

    private float nextMoleClickTime = 0.0f;
    private GameController gameController;
    //private List<SpriteRenderer> daylightEffectedObjects = new List<SpriteRenderer>();

    public FruitController[] fruits;

    private void Start()
    {
        GameObject[] _beeSpawnpoints = GameObject.FindGameObjectsWithTag("BeeSpawnPoint");
        beeSpawnpoints = new SpawnpointInGrass[_beeSpawnpoints.Length];

        for (int i = 0; i < _beeSpawnpoints.Length; i++)
        {
            beeSpawnpoints[i] = _beeSpawnpoints[i].GetComponent<SpawnpointInGrass>();
        }

        // -- Calculate DayTime --
        if (daynight)
        {
            System.TimeSpan start = System.TimeSpan.Parse(startOfNight);
            System.TimeSpan end = System.TimeSpan.Parse(endOfNight);
            System.TimeSpan now = System.DateTime.Now.TimeOfDay;

            if (start <= end)
            {
                // start and stop times are in the same day
                if (now >= start && now <= end)
                {
                    // current time is between start and stop
                    isNight = true;
                }
            }
            else
            {
                // start and stop times are in different days
                if (now >= start || now <= end)
                {
                    // current time is between start and stop
                    isNight = true;
                }
            }
        }

        // -- Setup Sky --
        isNight = forceNight ? true : isNight;

        if (isNight)
        {
            moon.SetActive(false);
            InitNightMode();
            //colorCorrectionCurves.enabled = true;  
        }
        else
        {
            moon.SetActive(false);
            sun.SetActive(true);
            sky.sprite = daySky;
            //colorCorrectionCurves.enabled = false;  
        }
    }

    public void InitNightMode()
    {
        moon.SetActive(true);
        sun.SetActive(false);
        sky.sprite = nightSky;

        SpriteRenderer[] sprites = FindObjectsOfType<SpriteRenderer>();

        for (int i = 0; i < sprites.Length; i++)
        {
            if (!sprites[i].CompareTag("IgnoneDayNight"))
            {
                sprites[i].color = nightTint;
            }
        }

        isNight = true;
        sprites = null;
    }

    public void InitDayMode()
    {
        moon.SetActive(false);
        sun.SetActive(true);
        sky.sprite = daySky;

        SpriteRenderer[] sprites = FindObjectsOfType<SpriteRenderer>();

        for (int i = 0; i < sprites.Length; i++)
        {
            if (!sprites[i].CompareTag("IgnoneDayNight"))
            {
                sprites[i].color = Color.white;
            }
        }

        isNight = false;
        sprites = null;
    }

    public void StartMole()
    {
        moleActivated = true;
        moleNextSpawnTime = Time.time + moleSpawnTimeSpan;
    }

    public void StopMole()
    {
        moleActivated = false;
    }

    public void StartPollen()
    {
        pollenActivated = true;
        pollenNextSpawnTime = Time.time + pollenSpawnTimeSpan;
    }

    public void StopPollen()
    {
        pollenActivated = false;
    }

    public void StartBee()
    {
        beeActivated = true;
        beeNextSpawnTime = Time.time + beeSpawnTimeSpan;
    }

    public void StopBee()
    {
        beeActivated = false;
    }

    public void StartEarthWorm()
    {
        eathWormActivated = true;
        earthNextSpawnTime = Time.time + earthSpawnTimeSpan;
    }

    public void StopEarthWorm()
    {
        eathWormActivated = false;
    }


    protected void Update()
    {
        if (moleSpawn && moleActivated && !mMoleWorking && Time.time > moleNextSpawnTime)
        {
            // Spawn Mole
            SpawnMole(moleSpawnpoints[GetRandomMoleCheckpoint()]);
        }

        if (beeSpawn && beeActivated && Time.time > beeNextSpawnTime)
        {
            // Spawn Mole
            SpawnBee(beeSpawnpoints[GetRandomBeeCheckpoint()]);
        }

        if (pollenSpawn && pollenActivated && Time.time > pollenNextSpawnTime && pollenCount < maxPolleCount)
        {
            SpawnPolle();
        }

        if (eathWormSpawn && eathWormActivated && Time.time > earthNextSpawnTime)
        {
            if (fruits == null || fruits.Length <= 0)
            {
                GameObject[] _fruits = GameObject.FindGameObjectsWithTag("Fruit");
                fruits = new FruitController[_fruits.Length];

                for (int i = 0; i < _fruits.Length; i++)
                {
                    fruits[i] = _fruits[i].GetComponent<FruitController>();
                }
            }

            SpawnEathWorm(fruits[GetRandomFruitCheckpoint()]);
        }

        if (mMole != null && mMole.molebodyCollider.enabled == false && Time.time > nextMoleClickTime)
        {
            mMole.molebodyCollider.enabled = true;
        }
    }

    #region Mole

    private void SpawnMole(SpawnpointInGrass target)
    {
        mMoleWorking = true;
        mMole.isDigging = false;
        mMole.transform.position = target.transform.position;
        mMole.transform.rotation = target.transform.rotation;
        mMole.SetSortingLayer(target.sortingLayerName);

        mMole.ShowSprites();
        //mMole.Appear(AppearComplete);
    }

    private void AppearComplete()
    {
        StartCoroutine(DoFunctionWithDelay(StartDigging, Random.Range(mMole.minMoleStayTime, mMole.maxMoleStayTime)));
    }

    private int GetRandomMoleCheckpoint()
    {
        int newIndex = Random.Range(0, moleSpawnpoints.Length);

        while (newIndex == lastMoleIndex)
        {
            newIndex = Random.Range(0, moleSpawnpoints.Length);
        }

        lastMoleIndex = newIndex;
        return lastMoleIndex;
    }

    public void StartDigging()
    {
        mMole.Dig(DigComplete);
    }

    private void DigComplete()
    {
        SetMoleWorkingComplete();
    }

    private void SetMoleWorkingComplete()
    {
        mMoleWorking = false;
        moleNextSpawnTime = Time.time + moleSpawnTimeSpan;
    }

    private IEnumerator DoFunctionWithDelay(FruitHighlightCallback method, float delay)
    {
        yield return new WaitForSeconds(delay);
        method();
    }

    #endregion

    #region Pollen

    public void SpawnPolle()
    {
        Vector3 spawnPoint = Camera.main.ViewportToWorldPoint(new Vector3(Random.Range(0.0f, 1.0f), Random.Range(0.4f, 1.0f), 20.0f));
        GameObject spawnedPolle = GameObject.Instantiate(pollenPrefab, spawnPoint, Quaternion.identity) as GameObject;

        spawnedPolle.GetComponentInChildren<SpriteRenderer>().sortingLayerName = spawnSortingLayers[Random.Range(0, spawnSortingLayers.Length)];
        spawnedPolle.GetComponent<PollenAnimation>().environmentController = this;

        if (isNight)
        {
            spawnedPolle.GetComponentInChildren<SpriteRenderer>().color = nightTint;
        }

        pollenNextSpawnTime = Time.time + pollenSpawnTimeSpan;
        ++pollenCount;
    }

    public void DecreasePolle()
    {
        --pollenCount;
    }

    #endregion

    #region Bee

    public void SpawnBee(SpawnpointInGrass target)
    {
        GameObject spawnedBee = GameObject.Instantiate(beePrefab, target.transform.position, Quaternion.identity) as GameObject;
        spawnedBee.GetComponent<SpriteRenderer>().sortingLayerName = target.sortingLayerName;

        if (isNight)
        {
            spawnedBee.GetComponent<SpriteRenderer>().color = nightTint;
        }

        beeNextSpawnTime = Time.time + beeSpawnTimeSpan;
    }

    private int GetRandomBeeCheckpoint()
    {
        int newIndex = Random.Range(0, beeSpawnpoints.Length);

        while (newIndex == lastBeeIndex)
        {
            newIndex = Random.Range(0, beeSpawnpoints.Length);
        }

        lastBeeIndex = newIndex;
        return lastBeeIndex;
    }

    #endregion

    #region EarthWorm

    public void SpawnEathWorm(FruitController target)
    {
        Debug.Log(target.name);
        earthWorm.StartAppear(target);
        earthNextSpawnTime = Time.time + earthSpawnTimeSpan;
    }

    private int GetRandomFruitCheckpoint()
    {
        int newIndex = Random.Range(0, fruits.Length);

        Debug.Log(newIndex);
        lastFruitIndex = newIndex;
        return lastFruitIndex;
    }

    #endregion

    public void MoleClicked()
    {
        nextMoleClickTime = Time.time + 5.0f;
        mMole.molebodyCollider.enabled = false;

        if (mMole.appeared)
        {
            StartDigging();
        }
        else
        {
            mMole.Appear();
        }
    }
}