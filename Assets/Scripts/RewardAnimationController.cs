using UnityEngine;
using System.Collections;

public class RewardAnimationController : MonoBehaviour 
{
    public GameController gameController;
    public BasketController basketController;

    public Sprite[] seedSprites;

    public Animator generalAnimator;
    public Animator starAnimator;
    public GameObject[] seeds;

    public GameObject seedBag;
    public Transform[] seedBagDropPositions;
    public iTween.EaseType seedEaseType;

    public SpriteRenderer countBGSprite;
    public TextMesh countText;

    private int seedCount = 0;
    private int seedsArrivedCount = 0;
    private int avalibleSeedCount = 0;

    public Vector3 seedShakeItensity = Vector3.one;
    public float seedShakeTime = 10.0f;

    [Header("SFX")]
    public AudioClip reward_star_appears;
    public AudioClip reward_star_disappears;
    public AudioClip reward_sparkling_stars;
    public AudioClip reward_seed_harvest;
    public AudioClip reward_bag_appears;

    public AudioClip reward_cheer;

    private ScreenFadeCallBack completeCallback;

    public void StartRewardAnimation(ScreenFadeCallBack callback)
    {
        generalAnimator.enabled = true;
        completeCallback = callback;

        basketController.Shake(basketController.shakeIntensity, 0.2f);
        basketController.StartGlow();
    }

    public void PrepairSeeds(int count, int currentAvalibleSeedCount)
    {
        seedCount = count;
        avalibleSeedCount = currentAvalibleSeedCount;

        if(currentAvalibleSeedCount > 0)
        {
            countText.text = avalibleSeedCount.ToString(); 
            countBGSprite.gameObject.SetActive(true);
        }
        else
        {
            countBGSprite.gameObject.SetActive(false);
        }
    }

    public void EnableSeeds()
    {
        RandomizeBuiltinArray(seedSprites);

        for (int i = 0; i < seeds.Length; i++)
        {
            seeds[i].GetComponent<SpriteRenderer>().sprite = seedSprites[i < seedSprites.Length ? i : Random.Range(0, seedSprites.Length)];
            seeds[i].SetActive(i < seedCount ? true : false);
            ShakeThat(seeds[i], seedShakeItensity, seedShakeTime);
        }
        
        StartCoroutine(MakeSeedbagVisible());
    }

    public void ShakeThat(GameObject gO, Vector3 strength, float time, float delay=0.0f)
    {
        iTween.RotateAdd(gO, iTween.Hash(
            "amount", strength,
            "time", time,
            "delay", delay,
            "easetype", iTween.EaseType.linear,
            "looptype", iTween.LoopType.pingPong,
            "onComplete", "ShakeFinished",
            "onCompleteTarget", gameObject
        ));
    }

    public void ShakeFinished()
    {
        
    }

    public void StopShakeThat(GameObject gO)
    {
        iTween.Stop(gO);
    }

    public IEnumerator MakeSeedbagVisible()
    {
        yield return new WaitForSeconds(1.0f);  
        seedBag.SetActive(true);

        if(completeCallback != null)
        {
            completeCallback();
        }
    }

    public void StartSeedIntoBackAnimation()
    {
        Animator seedAnimator;
        float delay = 0.0f;
        seedsArrivedCount = 0;

        for(int i = 0; i < seeds.Length; i++)
        {
            if(i < seedCount)
            {
                MoveTo(seeds[i], seedBagDropPositions, 0.5f, delay++);

                seedAnimator = seeds[i].GetComponent<Animator>();

                if(seedAnimator != null)
                {
                    seedAnimator.enabled = false;
                }
            }
        }
    }

    private void MoveTo(GameObject movedObject, Transform[] path, float time, float delay=0.0f)
    {
        StopShakeThat(movedObject);

        iTween.MoveTo(movedObject, iTween.Hash(
            "path", path,
            "time", time,
            "delay", delay,
            "easetype", seedEaseType,
            "onComplete", "SeedIsArrived",
            "onCompleteTarget", gameObject
        ));

        iTween.ScaleTo(movedObject, iTween.Hash(
            "x", 0.0f,
            "y", 0.0f,
            "z", 0.0f,
            "time", time,
            "delay", delay + 0.015f,
            "easetype", "linear"
        ));

        StartCoroutine(DoFunctionWithDelay(PlayRewardSeedHarvest, delay + 0.15f));
    }

    private IEnumerator DoFunctionWithDelay(FruitHighlightCallback method, float delay)
    {
        yield return new WaitForSeconds(delay);
        method();
    }

    private void SeedIsArrived()
    {
        ++seedsArrivedCount;
        ++avalibleSeedCount;
        countText.text = avalibleSeedCount.ToString();

        seedBag.GetComponent<Animator>().SetTrigger("Popping");

        if(seedsArrivedCount == seedCount)
        {
            PlayRewardStarDisappears();
            basketController.StopShake();
            basketController.StopGlow();
            gameController.BackToMenu(1.5f);
        }
    }

    public void PlayRewardStarAppears()
    {
        AudioManager.Instance.FadeOut(AudioManager.Instance.BackgroundChannel, 2.0f);

        AudioManager.Instance.SetSFXChannel(reward_star_appears, PlayRewardSparklingStars, 0, 0);
        AudioManager.Instance.SFXChannels[0].loop = false;

        AudioManager.Instance.SetSFXChannel(reward_cheer, null, 0, 3);
        AudioManager.Instance.SFXChannels[3].loop = false;
    }

    public void PlayRewardStarDisappears()
    {
        AudioManager.Instance.SetSFXChannel(reward_star_disappears, null, 0, 0);
        AudioManager.Instance.SFXChannels[0].loop = false;

        AudioManager.Instance.FadeOut(AudioManager.Instance.SFXChannels[1], 1.0f);
    }

    public void PlayRewardSparklingStars()
    {
        AudioManager.Instance.SetSFXChannel(reward_sparkling_stars, null, 0, 1);
        AudioManager.Instance.SFXChannels[1].loop = true;
    }

    public void PlayRewardSeedHarvest()
    {
        AudioManager.Instance.SetSFXChannel(reward_seed_harvest, null, 0, 2);
        AudioManager.Instance.SFXChannels[2].loop = false;
    }

    public void PlayBagAppears()
    {
        AudioManager.Instance.SetSFXChannel(reward_bag_appears, null, 0, 2);
        AudioManager.Instance.SFXChannels[2].loop = false;
    }

    private static void RandomizeBuiltinArray(Object[] array)
    {
        for (var i = array.Length - 1; i > 0; i--) 
        {
            var r = Random.Range(0,i);
            Object tmp = array[i];
            array[i] = array[r];
            array[r] = tmp;
        }
    }
}
