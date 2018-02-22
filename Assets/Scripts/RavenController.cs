using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class RavenController : MonoBehaviour 
{
	private Animator animator;
	private UnityAction curCallback;
    public UnityAction OnFishingLoteGrab;
    public UnityAction OnWhileFishingLoop;

    private Renderer[] renderers;

    public Animator fishingRod;

    private string[] startupAnimations = new string[] { "LoopingAndLand", "Fly_Land" };

    private string[] idleAnimations = new string[] {"Idle", "IdleGroovie" };
    private string[] idleSingingAnimations = new string[] {"IdleDefaultSinging", "IdleGroovingSinging" };

    private string[] appearAnimations = new string[] { "AppearBehindGrass01", "AppearBehindGrass02", "StairsUp" }; // "AppearBehindGrass01", "Fly_Land"
    private string[] diveAnimations = new string[] {
        "BigStepForward",
        "DigAndDissappear", 
        "Dive",
        "DiveAhead",
        "SecretAgentFlip",
        "StairsDown", 
        "Stumble",
        "WhistleAndJump",
        "DownTheDrain"
    }; // "Land_Fly"

    private string[] pokeAnimations = new string[] { "Poke01", "Poke02", "Poke03" };
    private string[] scriptedPokeAnimations = new string[] { "Poke01", "Poke01", "Poke01", "Poke02", "Poke03", "RantFlyOff" };
    private string[] scriptedGamePokeAnimations = new string[] { "Poke01", "Poke01", "Poke01", "Poke02", "Poke03" };
    private int scriptedPokeIndex = 0;

    private string[] ravenLoss = new string[] { "PoutAndStomp", "RantFlyOff" };

    private AudioClip curAudioClip = null;

	public BasketController basketController;
    public GameObject remoteControl;

	[Header("SFX")]
    public AudioClip[] sfx_appear_behind_grass_1;
    public AudioClip[] sfx_appear_behind_grass_2;
    public AudioClip[] sfx_appear_behind_tree;

    public AudioClip[] sfx_basket_and_run;
    public AudioClip[] sfx_big_step_forward;

    public AudioClip[] sfx_dig_and_dissapear;
    public AudioClip[] sfx_dive;
    public AudioClip[] sfx_dive_ahead;
    public AudioClip[] sfx_duck_and_cover;
    public AudioClip[] sfx_down_the_drain;

    public AudioClip[] sfx_idle_grooving_singing;
    public AudioClip[] sfx_idle_default_singing;

    public AudioClip[] sfx_fishing_start;
    public AudioClip[] sfx_fishing_loop;
    public AudioClip[] sfx_fishing_end;
    public AudioClip[] sfx_fly_and_land;

    public AudioClip[] sfx_looping_and_land;
    public AudioClip[] sfx_looping_and_land_tutorial;

    public AudioClip[] sfx_pound_and_stomp;
    public AudioClip[] sfx_rant_and_fly_off;
    
    public AudioClip[] sfx_secret_agent_flip;
    public AudioClip[] sfx_stairs_down;
    public AudioClip[] sfx_stairs_up;
    public AudioClip[] sfx_stumble;

    public AudioClip[] sfx_throw;
    public AudioClip[] sfx_whistle_and_jump;

    public AudioClip[] sfx_drum_boing;

    public AudioClip[] sfx_poke_01;
    public AudioClip[] sfx_poke_02;
    public AudioClip[] sfx_poke_03;

    [Header("Fruit Drop at the End")]
	private Sprite[] fruitSprites;

	public Transform fruitSpawn;
	public Transform fruitDestination;

    public GameObject fruitInstancePrefab;

    private float startDelay = 0.0f;
    private bool tutorial = false;
    private bool atTheBasket = false;

    public Transform angryParticleSpot;

    [Header("PaticleEffects")]
    public ParticleSystem particle_digging;
    public ParticleSystem particle_diving;
    public ParticleSystem particle_ravenAngry;

    private bool idleBlocked = false;
    public bool blockRandomIdlePermanent = false;

    public float singingPosibility = 0.1f;

    public bool isIdling
    {
        get
        {
            return !idleBlocked;
        }

        private set
        {
            
        }
    }

/*
    Start - LoopingAndLand

    Idle
    -------
    Idle-
    IdleAtBasket-special
    IdleGroovie-

    Apear
    -------
    AppearBehindGrass01-
    AppearBehindGrass02-
    Appear-
    Fly_Land-

    Dive
    -------
    Land_Fly-
    Dive-
    SecretAgentFlip-
    StairsDown-
    RantFlyOff--
    DuckAndCover-
    DigAndDissappear-
    WhistleAndJump-

    Minigames
    ----------
    Throw
    UseRemote
    UseFishing

    Lose - BasketAndRun
    Win - PoutAndStomp
*/

	private void Awake()
	{
		animator = GetComponent<Animator>();
        renderers = GetComponentsInChildren<Renderer>();
	}

    private void OnDestroy()
    {
        OnFishingLoteGrab = null;
    }
	
    #region general animation functions

    public void StartUp(UnityAction callback)
    {
        CallAnimation(startupAnimations[Random.Range(0, startupAnimations.Length)], callback);
    }

    public void Idle()
    {
        Idle(null);
    }

    public void Idle(UnityAction callback)
    {
        CallAnimation(idleAnimations[Random.Range(0, idleAnimations.Length)], callback);
    }

    public void IdleSinging(UnityAction callback)
    {
        CallAnimation(idleSingingAnimations[Random.Range(0, idleSingingAnimations.Length)], callback);
    }

    public void Appear(UnityAction callback)
    {
        CallAnimation(appearAnimations[Random.Range(0, appearAnimations.Length)], callback);
    }

    public void Dive(int ravenPosition, UnityAction callback)
    {
        int random = Random.Range(0, diveAnimations.Length);

        SetParticleSortingLayerByName(diveAnimations[random], ravenPosition);
        CallAnimation(diveAnimations[random], callback);
    }

    public void RavenLoss(UnityAction callback)
    {
        CallAnimation(ravenLoss[Random.Range(0, ravenLoss.Length)], callback);
    }

    public void Poke(UnityAction callback)
    {
        AudioManager.Instance.SetSFXChannel(null);
        CallAnimation(pokeAnimations[Random.Range(0, pokeAnimations.Length)], callback);
    }

    public void ScripedPoke()
    {
        ScripedPoke(null);
    }

    public void ScripedPoke(UnityAction callback)
    {
        if(scriptedPokeIndex >= scriptedPokeAnimations.Length)
            return;

        if(scriptedPokeIndex == scriptedPokeAnimations.Length - 1)
        {
            callback = null;
        }
        
        AudioManager.Instance.SetSFXChannel(null);
        CallAnimation(scriptedPokeAnimations[scriptedPokeIndex++], callback);
    }

    public void ScripedPokeGameBlank()
    {
        ScripedPokeGame(null);
    }

    public void ScripedPokeGame(UnityAction callback)
    {
        AudioManager.Instance.SetSFXChannel(null);

        if(scriptedPokeIndex < scriptedGamePokeAnimations.Length)
        {
            CallAnimation(scriptedGamePokeAnimations[scriptedPokeIndex++], callback);
        }
        else
        {
            CallAnimation(pokeAnimations[Random.Range(0, pokeAnimations.Length)], callback);
        }
    }

    #endregion

    #region specific animation functions

    public void AppearBehindGrass01(UnityAction callback)
    {
        CallAnimation("AppearBehindGrass01", callback);
    }

    public void AppearBehindGrass02(UnityAction callback)
    {
        CallAnimation("AppearBehindGrass02", callback);
    }

    public void BasketAndRun(UnityAction callback)
    {
        CallAnimation("BasketAndRun", callback);
    }

    public void BigStepForward(UnityAction callback)
    {
        CallAnimation("BigStepForward", callback);
    }

    public void DigAndDissappear(UnityAction callback)
    {
        CallAnimation("DigAndDissappear", callback);
    }

    public void DuckAndCover(UnityAction callback)
    {
        CallAnimation("DuckAndCover", callback);
    }

    public void DiveAhead(UnityAction callback)
    {
        CallAnimation("DiveAhead", callback);
    }

    public void DownTheDrain(UnityAction callback)
    {
        CallAnimation("DownTheDrain", callback);
    }

    public void StartFishing(UnityAction callback)
    {
        CallAnimation("StartFishing", callback);
        fishingRod.SetTrigger("StartFishing");
    }

    public void WhileFishingLoop()
    {
        SetSFXAudioClip("LoopFishing");
        PlaySFXForCurrentAnimation();

        if(OnWhileFishingLoop != null)
        {
            OnWhileFishingLoop();
        }
    }

    public void StopFishing(UnityAction callback)
    {
        CallAnimation("StopFishing", callback);
        fishingRod.SetTrigger("StopFishing");
    }

    public void StartFishing()
    {
        CallAnimation("StartFishing", null);
        fishingRod.SetTrigger("StartFishing");
    }

    public void StopFishing()
    {
        CallAnimation("StopFishing", null);
        fishingRod.SetTrigger("StopFishing");
    }

    public void FlyDown(UnityAction callback)
    {
        CallAnimation("Fly_Land", callback);
    }

    public void FlyUp(UnityAction callback)
    {
        CallAnimation("Land_Fly", callback);
    }

    public void SimpleIdle(UnityAction callback)
    {
        CallAnimation("Idle", callback);
    }

    public void IdleGroovie(UnityAction callback)
    {
        CallAnimation("IdleGroovie", callback);
    }

    public void IdleAtBasket()
    {
        IdleAtBasket(null);
    }

    public void IdleAtBasket(UnityAction callback)
    {
        CallAnimation("IdleAtBasket", callback);
    }

    public void IdleDefaultSinging(UnityAction callback)
    {
        CallAnimation("IdleDefaultSinging", callback);
    }

    public void IdleGroovingSinging(UnityAction callback)
    {
        CallAnimation("IdleGroovingSinging", callback);
    }

    public void StartRemote(UnityAction callback)
    {
        CallAnimation("StartRemote", true, callback);
        remoteControl.SetActive(true);
    }

    public void StopRemote(UnityAction callback)
    {
        CallAnimation("StopRemote", callback);
        remoteControl.SetActive(false);
    }
       
    public void SimpleDive(UnityAction callback)
    {
        CallAnimation("Dive", callback);
    }

    public void LoopingAndLand(UnityAction callback)
    {
        CallAnimation("LoopingAndLand", callback);
    }

    public void PoutAndStomp(UnityAction callback)
    {
        CallAnimation("PoutAndStomp", callback);
    }

    public void Pout(UnityAction callback)
    {
        CallAnimation("Pout", callback);
    }

    public void RantFlyOff(UnityAction callback)
    {
        CallAnimation("RantFlyOff", callback);
    }

    public void SecretAgentFlip(UnityAction callback)
    {
        CallAnimation("SecretAgentFlip", callback);
    }

    public void SimpleAppear(UnityAction callback)
    {
        CallAnimation("Appear", callback);
    }

    public void StairsDown(UnityAction callback)
    {
        CallAnimation("StairsDown", callback);
    }

    public void StairsUp(UnityAction callback)
    {
        CallAnimation("StairsUp", callback);
    }

    public void Stumble(UnityAction callback)
    {
        CallAnimation("Stumble", callback);
    }

    public void Throw(UnityAction callback)
    {
        CallAnimation("Throw", callback);
    }

    public void WhistleAndJump(UnityAction callback)
    {
        CallAnimation("WhistleAndJump", callback);
    }

    public void Poke01(UnityAction callback)
    {
        CallAnimation("Poke01", callback);
    }

    public void Poke02(UnityAction callback)
    {
        CallAnimation("Poke02", callback);
    }

    public void Poke03(UnityAction callback)
    {
        CallAnimation("Poke03", callback);
    }

    #endregion
	

    #region General functions

    /// <summary>
    /// Calls a trigger parameter animation.
    /// </summary>
    /// <param name="parameterName">Parameter name.</param>
    /// <param name="callback">Callback.</param>
    public void CallAnimation(string parameterName, UnityAction callback)
    {
        Debug.Log("DoAnimation: " + parameterName);
        SetSFXAudioClip(parameterName);
        animator.SetTrigger(parameterName);

        curCallback = callback;

        PlaySFXForCurrentAnimation(startDelay);
    }

    /// <summary>
    /// Calls a bool parameter animation.
    /// </summary>
    /// <param name="parameterName">Parameter name.</param>
    /// <param name="status">If set to <c>true</c> status.</param>
    /// <param name="callback">Callback.</param>
    public void CallAnimation(string parameterName, bool status, UnityAction callback)
    {
        Debug.Log("DoAnimation: " + parameterName);
        SetSFXAudioClip(parameterName);
        animator.SetBool(parameterName, status);

        curCallback = callback;

        PlaySFXForCurrentAnimation(startDelay);
    }

    /// <summary>
    /// Animations the complete.
    /// </summary>
    public void AnimationComplete()
    {
        if (curCallback != null) 
        {
            curCallback();
        }

        idleBlocked = false;
    }

    public void DoVisible()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].enabled = true;
        }
    }

    public void DoInvisible()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].enabled = false;
        }
    }
    
    public void SetSFXAudioClip(string parameterName)
    {
        startDelay = 0.0f;
        idleBlocked = true;

        switch(parameterName)
        {
            case "AppearBehindGrass01":
                curAudioClip = GetRandomClipOfAudioArray(sfx_appear_behind_grass_1);
                break;
            case "AppearBehindGrass02":
                curAudioClip = GetRandomClipOfAudioArray(sfx_appear_behind_grass_2);
                break;
            case "AppearBehindTree":
                curAudioClip = GetRandomClipOfAudioArray(sfx_appear_behind_tree);
                break;
            case "BasketAndRun":
                curAudioClip = GetRandomClipOfAudioArray(sfx_basket_and_run);
                break;
            case "BigStepForward":
                curAudioClip = GetRandomClipOfAudioArray(sfx_big_step_forward);
                break;
            case "DigAndDissapear":
                curAudioClip = GetRandomClipOfAudioArray(sfx_dig_and_dissapear);
                break;
            case "Dive":
                curAudioClip = GetRandomClipOfAudioArray(sfx_dive);
                break;
            case "DiveAhead":
                curAudioClip = GetRandomClipOfAudioArray(sfx_dive_ahead);
                break;
            case "DownTheDrain":
                curAudioClip = GetRandomClipOfAudioArray(sfx_down_the_drain);
                break;
            case "StartFishing":
                curAudioClip = GetRandomClipOfAudioArray(sfx_fishing_start);
                break;
            case "LoopFishing":
                curAudioClip = GetRandomClipOfAudioArray(sfx_fishing_loop);
                break;
            case "StopFishing":
                curAudioClip = GetRandomClipOfAudioArray(sfx_fishing_end);
                break;
            case "Fly_Land":
                curAudioClip = GetRandomClipOfAudioArray(sfx_fly_and_land);
                startDelay = 2.75f;
                break;
            case "Idle":  
                idleBlocked = false;
                break;
            case "IdleGroovie":  
                idleBlocked = false;
                break;
            case "IdleGroovingSinging":
                curAudioClip = GetRandomClipOfAudioArray(sfx_idle_grooving_singing);
                idleBlocked = false;
                break;
            case "IdleDefaultSinging":
                curAudioClip = GetRandomClipOfAudioArray(sfx_idle_default_singing);
                idleBlocked = false;
                break;  
            case "LoopingAndLand":

                if (tutorial)
                {
                    // Tutorial Mode
                    curAudioClip = GetRandomClipOfAudioArray(sfx_looping_and_land_tutorial);
                }
                else
                {
                    // Normnal Mode
                    curAudioClip = GetRandomClipOfAudioArray(sfx_looping_and_land);
                }

                break;
            case "Poke01":
                curAudioClip = GetRandomClipOfAudioArray(sfx_poke_01);
                break;
            case "Poke02":
                curAudioClip = GetRandomClipOfAudioArray(sfx_poke_02);
                break;
            case "Poke03":
                curAudioClip = GetRandomClipOfAudioArray(sfx_poke_03);
                break;
            case "PoutAndStomp":
                curAudioClip = GetRandomClipOfAudioArray(sfx_pound_and_stomp);
                break;
            case "RantFlyOff":
                curAudioClip = GetRandomClipOfAudioArray(sfx_rant_and_fly_off);
                break;
            case "SecretAgentFlip":
                curAudioClip = GetRandomClipOfAudioArray(sfx_secret_agent_flip);
                break;
            case "StairsDown":
                curAudioClip = GetRandomClipOfAudioArray(sfx_stairs_down);
                break;
            case "StairsUp":
                curAudioClip = GetRandomClipOfAudioArray(sfx_stairs_up);
                break;
            case "Stumble":
                curAudioClip = GetRandomClipOfAudioArray(sfx_stumble);
                break;
            case "Throw":
                curAudioClip = GetRandomClipOfAudioArray(sfx_throw);
                break;  
            case "WhistleAndJump":
                curAudioClip = GetRandomClipOfAudioArray(sfx_whistle_and_jump);
                break;
            default:
                break;
        }

        Debug.Log(parameterName + ": " + idleBlocked);
    }

    public AudioClip GetRandomClipOfAudioArray(AudioClip[] clips)
    {
        if(clips == null || clips.Length <= 0)
            return null;

        return clips[Random.Range(0, clips.Length)];
    }

    public void PlaySFXForCurrentAnimation(float sfxDelay = 0.0f)
    {
        if(curAudioClip != null)
        {
            AudioManager.Instance.SetSFXChannel(curAudioClip, null, sfxDelay);
            curAudioClip = null;
        }
    }

	public void ShootFruits()
	{
		StartCoroutine(ShootingFruits());
	}

	private IEnumerator ShootingFruits()
	{
		GameObject newInstance = null;
        MeshRenderer fruitRenderer = null;
        SpriteRenderer spriteRenderer = null;
        Texture texture = null;

		for(int i = 0; i < fruitSprites.Length; i++) 
		{
			if(fruitSprites[i] != null) 
			{
                //newInstance = new GameObject("FlyingFruit", typeof(SpriteRenderer), typeof(Rigidbody)); 
                newInstance = GameObject.Instantiate(fruitInstancePrefab);

                //fruitRenderer = newInstance.GetComponent<MeshRenderer>();
                //fruitRenderer.material.mainTexture = fruitSprites[i].texture;

                spriteRenderer = newInstance.GetComponent<SpriteRenderer>();
				spriteRenderer.sprite = fruitSprites[i];
				spriteRenderer.sortingLayerName = "GrassLayer2";
				
                newInstance.transform.localPosition = Vector3.zero + (Vector3.forward * 5.0f);
				newInstance.transform.localScale = Vector3.one * 0.5f;

                float forceUp = 8.0f;
                float forceHorizontal = 2.5f;
                float forceDepth = 2.5f;

				newInstance.GetComponent<Rigidbody>().AddForce(new Vector3(
                    (Random.Range(0, 2) == 1 ? 1 : -1) * forceHorizontal, 
                    forceUp, 
                    Random.Range(0, 2) == 1 ? 1 : -1) * forceDepth, 
					ForceMode.Impulse);
				
				yield return new WaitForSeconds (0.33f);
			}
		}
	}

	public void FruitShootComplete()
	{
		Debug.Log("Fruit is shooted complete");
	}

	public void ClearBasket()
	{
		fruitSprites = basketController.GetFruitSprites();
		basketController.ClearBasket();
	}

    public void Activate()
    {
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void RandomIdle()
    {
        if(blockRandomIdlePermanent || idleBlocked)
            return;

        if(atTheBasket)
        {
            IdleAtBasket();
        }
        else
        {
            if(tutorial)
            {
                SimpleIdle(null);
            }
            else
            {
                if(Random.Range(0.0f, 1.0f) <= singingPosibility)
                {
                    IdleSinging(null);
                }
                else
                {
                    Idle(null);
                }
            }
        }
    }

    #endregion

    #region Tutorial Mode

    public void SetTutorialMode(bool status)
    {
        tutorial = status;
    }

    public void SetAtTheBasket(bool status)
    {
        atTheBasket = status;
    }

    #endregion

    #region ParticleSystem

    public void StartParticleDigging(float duration)
    {
        ParticleSystem partSys = ParticleSystem.Instantiate(particle_digging, transform.position, transform.rotation) as ParticleSystem;
        StartCoroutine(ParticleEffectRunning(partSys, duration));
        Destroy(partSys.gameObject, 10.0f);
    }

    public void StartParticleDiving(float duration)
    {
        ParticleSystem partSys = ParticleSystem.Instantiate(particle_diving, transform.position, transform.rotation) as ParticleSystem;
        StartCoroutine(ParticleEffectRunning(partSys, duration));
        Destroy(partSys.gameObject, 10.0f);
    }

    public void StartParticleAngry(float duration)
    {
        ParticleSystem partSys = ParticleSystem.Instantiate(particle_ravenAngry, angryParticleSpot.position, angryParticleSpot.rotation) as ParticleSystem;
        StartCoroutine(ParticleEffectRunning(partSys, duration));
        Destroy(partSys.gameObject, 10.0f);
    }

    private IEnumerator ParticleEffectRunning(ParticleSystem particleSystem, float duration)
    {
        particleSystem.Play();

        yield return new WaitForSeconds(duration);

        if(particleSystem != null)
            particleSystem.Stop();
    }

    private void ClearParticles()
    {
        particle_digging.Clear();
        particle_diving.Clear();
    }

    public void SetParticleSortingLayer(int ravenPosition)
    {
        string sortingLayerName = "Default";

        ParticleSystemRenderer prDig = particle_digging.GetComponent<Renderer>() as ParticleSystemRenderer;
        ParticleSystemRenderer prDive = particle_diving.GetComponent<Renderer>() as ParticleSystemRenderer;

        switch(ravenPosition)
        {
            case 0:
                sortingLayerName = "Background";
                break;
            case 1:
                sortingLayerName = "GrassLayer5";
                break;
            case 2:
                sortingLayerName = "GrassLayer4";
                break;
            case 3:
                sortingLayerName = "GrassLayer3";
                break;
            case 4:
                sortingLayerName = "GrassLayer2";
                break;
            case 5:
                sortingLayerName = "GrassLayer1";
                break;
            default:
                break;
        }

        //Debug.Log(sortingLayerName);
        prDig.sortingLayerName = prDive.sortingLayerName = sortingLayerName;
    }

    public void SetParticleSortingLayerByName(string animationName, int ravenPosition)
    {
        switch(animationName)
        {
            case "Stumble":
                ++ravenPosition;
                break;
            case "DiveAhead":
                ++ravenPosition;
                break;
            case "WhistleAndJump":
                ++ravenPosition;
                break;
            default:
                break;
        }

        SetParticleSortingLayer(ravenPosition);
    }


    public void PlayDrumBoing()
    {
        AudioManager.Instance.SetSFXChannel(sfx_drum_boing[Random.Range(0, sfx_drum_boing.Length)], null, 0, 2);
    }

    public void FishingLoteGrab()
    {
        if(OnFishingLoteGrab != null)
        {
            OnFishingLoteGrab();
        }
    }

    public void SetIdleStatus(bool status)
    {
        idleBlocked = !status;
    }

    #endregion
}