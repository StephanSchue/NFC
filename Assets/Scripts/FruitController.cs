using UnityEngine;
using System.Collections;

public class FruitController : MonoBehaviour 
{
	[HideInInspector()] private Animator animator;
    [HideInInspector()] public SpriteRenderer spriteRender;
    public string oldSortingLayer { get; private set; }

    public SpriteRenderer glowRenderer;
    public SpriteRenderer outlineRenderer;

    public Transform treeParent { get; private set; }
	public Vector3 originalPosition { get; private set; }
    public Vector3 originalLocalScale { get; private set; }

	public FruitColor iD;

    public bool inTheBasket { get; set; }

    public Vector3 offset = Vector3.zero;

    public ParticleSystem particleSystemLeafsFall;

	public Sprite curSprite 
	{
		get 
		{
			return spriteRender.sprite;
		}

		private set 
		{
			
		}
	}

    private int[] fruitRotations = new int[3];
    private float fruitRotationsTime = 0;
    private int fruitRotationsIndex = 0;
    private bool startRotation = false;
		
	/// <summary>
	/// Use this for initialization
	/// </summary>
	private void Start() 
	{
		spriteRender = GetComponent<SpriteRenderer>();
		spriteRender.sortingOrder = 10;
		animator = GetComponent<Animator>();

        treeParent = transform.parent;
        originalLocalScale = transform.localScale;
		originalPosition = transform.position;

        glowRenderer.transform.localPosition = outlineRenderer.transform.localPosition = (Vector3.down * 2.0f) + offset; // (spriteRender.bounds.center * spriteRender.transform.localScale.x) + 
        glowRenderer.sortingOrder = 5;

        transform.localRotation = Quaternion.identity;

        TutorialController.OnFruitHighlight += HighlightFruit;
        TutorialController.OnFruitUnHighlight += UnHighlightFruit;
        UnHighlightFruit();
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnDestroy() 
    {
        StopRotation();
        
        TutorialController.OnFruitHighlight -= HighlightFruit;
        TutorialController.OnFruitUnHighlight -= UnHighlightFruit;
    }

	public void ResetPosition()
	{
		transform.position = originalPosition;
	}

	public void Grapped()
	{
        originalPosition = transform.parent.position;

		oldSortingLayer = spriteRender.sortingLayerName;
		spriteRender.sortingLayerName = "GrappedFruit";

        ParticleSystemRenderer particleSystemLeafsFallRenderer = particleSystemLeafsFall.GetComponent<Renderer>() as ParticleSystemRenderer;
        particleSystemLeafsFallRenderer.sortingLayerName = oldSortingLayer;
        particleSystemLeafsFallRenderer.sortingOrder = spriteRender.sortingOrder - 1;

        StartParticleLeafsFallSystem(0.2f);

	}

	public void Leave()
	{
		spriteRender.sortingLayerName = oldSortingLayer;
	}

    public void DroppedAtBasket()
    {
        inTheBasket = true;
    }

    public void HighlightFruit()
    {
        if(glowRenderer != null)
        {
            glowRenderer.enabled = true;

            Animator glowAnimator = glowRenderer.gameObject.GetComponent<Animator>();

    		if(glowAnimator != null)
    		{
    			//glowAnimator.enabled = false;
    			glowAnimator.SetTrigger("Highlight");
    		}     
        }

        if (outlineRenderer != null)
        {
            outlineRenderer.enabled = true;
        }

        StartRotation();      
    }

    public void UnHighlightFruit()
    {
        if(glowRenderer != null)
        {
            glowRenderer.enabled = false;
        
            Animator glowAnimator = glowRenderer.gameObject.GetComponent<Animator>();

        	if(glowAnimator != null)
        	{
        		//glowAnimator.enabled = false;
        		glowAnimator.SetTrigger("UnHighlight");
        	}    
        }

        if (outlineRenderer != null)
        {
            outlineRenderer.enabled = false;
        }

        StopRotation();   
    }

    #region Rotation

    public void StartRotation()
    {
        int rotation = Random.Range(3, 11);

        fruitRotations = new int[3] { -rotation, rotation, 0 };
        fruitRotationsIndex = 0;
        fruitRotationsTime = Random.Range(0.5f, 1.3f);
        startRotation = true;

        DoRotations();
    }
        
    public void DoRotations()
    {
        if(!startRotation)
            return;
        
        iTween.RotateTo(gameObject, iTween.Hash(
            "x", 0.0f,
            "y", 0.0f,
            "z", fruitRotations[fruitRotationsIndex],
            "time", fruitRotationsTime, 
            "easetype", "linear",
            "onComplete", "DoRotations", 
            "onCompleteTarget", gameObject
        ));

        ++fruitRotationsIndex;

        if(fruitRotationsIndex >= fruitRotations.Length)
        {
            fruitRotationsIndex = 0;
        }
    }

    public void StopRotation()
    {
        startRotation = false;
        iTween.Stop(gameObject);
    }

    #endregion

    #region particle systems

    public void StartParticleLeafsFallSystem(float duration)
    {
        ParticleSystem partSys = ParticleSystem.Instantiate(particleSystemLeafsFall, transform.position, transform.rotation) as ParticleSystem;
        StartCoroutine(ParticleEffectRunning(partSys, duration));
        //Destroy(partSys, 10.0f);
    }

    private IEnumerator ParticleEffectRunning(ParticleSystem particleSystem, float duration)
    {
        particleSystem.Play();

        yield return new WaitForSeconds(duration);

        particleSystem.Stop();
    }

    #endregion
}
