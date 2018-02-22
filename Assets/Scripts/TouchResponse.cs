using UnityEngine;
using System.Collections;

public class TouchResponse : MonoBehaviour 
{
    private ParticleSystem[] particleSystems;
    private Animator animator;
    public bool useHitPosition = false;

    public GameObject geometryEffect;
    public float geometryEffectTime = 5.0f;
    public float geometryFadeOutTime = 2.0f;
    private SpriteRenderer geometryEffectSpriteRenderer;

    private Vector3 baseScale;
    public bool bounce = false;

    public AudioClip[] sfx;
    public AudioSource audioSource;

    private FlyingBee flyingBee;

	// Use this for initialization
	protected void Awake() 
    {
        baseScale = transform.localScale;
        particleSystems = GetComponentsInChildren<ParticleSystem>();
        animator = GetComponentInChildren<Animator>();
        flyingBee = GetComponent<FlyingBee>();
        
        if(geometryEffect != null)
        {
            geometryEffectSpriteRenderer = geometryEffect.GetComponent<SpriteRenderer>();
        }
	}

    public void StartParticleSystem(Vector3 position = default(Vector3))
    {
        //Debug.Log(gameObject.name);

        if(particleSystems != null && particleSystems.Length > 0)
        {
            for (int i = 0; i < particleSystems.Length; i++)
            {
                if(useHitPosition && position != Vector3.zero)
                {
                    particleSystems[i].transform.position = position; //new Vector3(Random.Range(-2.0f, 2.0f), Random.Range(-2.0f, 2.0f), Random.Range(-2.0f, 2.0f));
                }

                particleSystems[i].Play();
            }
        }

        if(geometryEffect != null)
        {
            EnableGeometryEffect();
            StartCoroutine(DoFunctionWithDelay(FadeOutGeometryEffect, geometryEffectTime));
        }

        if(animator != null)
        {
            animator.SetTrigger("Touch");
        }

        if(sfx != null && sfx.Length > 0)
        {
            if(audioSource != null)
            {
                audioSource.clip = sfx[Random.Range(0, sfx.Length)];
                audioSource.Play();
            }
            else
            {
                AudioManager.Instance.SetSFXChannel(sfx[Random.Range(0, sfx.Length)], null, 0, 1);
            }
        }

        if(flyingBee != null)
        {
            flyingBee.Tapped();
        }
    }

    private IEnumerator DoFunctionWithDelay(FruitHighlightCallback method, float delay)
    {
        yield return new WaitForSeconds(delay);
        method();
    }

    public void EnableGeometryEffect()
    {
        geometryEffect.SetActive(true);

        if(geometryEffectSpriteRenderer != null)
           geometryEffectSpriteRenderer.color = new Color(geometryEffectSpriteRenderer.color.r, geometryEffectSpriteRenderer.color.g, geometryEffectSpriteRenderer.color.b, 1.0f);
    }

    public void FadeOutGeometryEffect()
    {
        if(geometryEffectSpriteRenderer != null)
        {
            iTween.ValueTo(geometryEffect.gameObject, iTween.Hash(
                "from", 1.0f,
                "to", 0.0f,
                "time", geometryFadeOutTime,
                "delay", 0.0f,
                "easetype", "linear",
                "onComplete", "DisableGeometryEffext",
                "onCompleteTarget", gameObject,
                "onUpdate", "OnFade",
                "onUpdateTarget", gameObject
            ));
        }
        else
        {
            DisableGeometryEffect();
        }
    }

    public void OnFade(float value)
    {
        if(geometryEffectSpriteRenderer != null)
            geometryEffectSpriteRenderer.color = new Color(geometryEffectSpriteRenderer.color.r, geometryEffectSpriteRenderer.color.g, geometryEffectSpriteRenderer.color.b, value);
    }

    public void DisableGeometryEffect()
    {
        geometryEffect.SetActive(false);
    }
}
