using UnityEngine;
using System.Collections;

public class BasketController : MonoBehaviour 
{
	public Transform[] fruitsAnchor;
	public FruitController[] fruitsReference;
	public int iterator = 0;

	[HideInInspector()]
	public Sprite DebuggingFruit;
	public Transform fruitContainer;
    public GameObject glow;

    public Vector3 shakeIntensity = Vector3.one;
    public float time = 10.0f;

    private ScreenFadeCallBack shakeCallback;

	/// <summary>
	/// Update is called once per frame
	/// </summary>
	private void Start()
	{
		CollectPlaceholders();
		ResetBasket();
	}

	/// <summary>
	/// Add the fruit.
	/// </summary>
	public void AddFruit(FruitController fruitSprite)
	{
		if(iterator > fruitsAnchor.Length - 1)
			return;

		fruitsReference[iterator] = fruitSprite;
		iterator = iterator < fruitsAnchor.Length - 1 ? iterator+1 : iterator;
	}

    public FruitController GetLastFruit()
    {
        if(iterator <= 0)
            return null;

        return fruitsReference[iterator-1];   
    }

	/// <summary>
	/// Gets the last fruit anchor position.
	/// </summary>
	/// <returns>The last fruit anchor position.</returns>
	public Transform GetLastFruitAnchorPosition()
	{
		return fruitsAnchor[iterator];
	}

    /// <summary>
    /// Remove the fuit.
    /// </summary>
    public void RemoveFuit(FruitController controller)
    {
        controller.transform.parent = controller.treeParent;
        controller.transform.localRotation = Quaternion.identity;
        controller.transform.localScale = controller.originalLocalScale;
        controller.spriteRender.sortingLayerName = controller.oldSortingLayer;

        Collider2D col2D = controller.GetComponent<Collider2D>();

        if(col2D != null)
        {
            col2D.enabled = true;
        }

        controller.inTheBasket = false;

        iTween.MoveTo(controller.gameObject, iTween.Hash(
            "x", controller.originalPosition.x,
            "y", controller.originalPosition.y,
            "z", controller.originalPosition.z,
            "time", 0.7f,
            "easetype", "easeOutQuad",
            "onComplete", "FruitIsComplete",
            "onCompleteTarget", gameObject
        ));

        RemoveFuit();
    }
    
	/// <summary>
	/// Remove the fuit.
	/// </summary>
	public void RemoveFuit()
	{
		if (iterator < 0)
			return;

		fruitsReference[iterator] = null;
		iterator = iterator > 0 ? iterator-1 : iterator;
	}

	public void ClearBasket()
	{
		for (int i = 0; i < fruitsReference.Length; i++) 
		{
			if (fruitsReference[i] != null) 
			{
				Destroy(fruitsReference[i].gameObject);
				fruitsReference[i] = null;
			}
		}
	}

	/// <summary>
	/// Reset the basket.
	/// </summary>
	public void ResetBasket()
	{
		if (fruitsReference == null || fruitsReference.Length <= 0)
			return;

		for (int i = 0; i < fruitsReference.Length; i++) 
		{
			if (fruitsReference [i] != null) 
			{
				fruitsReference [i].ResetPosition ();
				fruitsReference [i] = null;
			}
		}
			
		iterator = 0;
	}

	public Sprite[] GetFruitSprites()
	{
		Sprite[] sprites = new Sprite[fruitsReference.Length];

		if (fruitsReference == null || fruitsReference.Length <= 0)
			return null;

		for (int i = 0; i < fruitsReference.Length; i++) 
		{
			if (fruitsReference[i] != null) 
			{
				sprites[i] = fruitsReference[i].curSprite;
			}
		}

		return sprites;
	}

	/// <summary>
	/// Collect the placeholders for the fruits
	/// </summary>
	public void CollectPlaceholders()
	{
		fruitsAnchor = fruitContainer.GetComponentsInChildren<Transform>();
		fruitsReference = new FruitController[fruitsAnchor.Length];
	}

    public void Shake(Vector3 strength, float time, float delay=0.0f, ScreenFadeCallBack callback = null)
    {
        shakeCallback = callback;

        iTween.RotateAdd(gameObject, iTween.Hash(
            "amount", strength,
            "time", time,
            "delay", delay,
            "looptype", iTween.LoopType.pingPong,
            "onComplete", "ShakeFinished",
            "onCompleteTarget", gameObject
        ));
    }

    public void RealShake(Vector3 strength, float time, float delay=0.0f, ScreenFadeCallBack callback = null)
    {
        shakeCallback = callback;

        iTween.ShakeRotation(gameObject, iTween.Hash(
            "amount", strength,
            "time", time,
            "delay", delay,
            "onComplete", "ShakeFinished",
            "onCompleteTarget", gameObject
        ));
    }

    public void StopShake()
    {
        iTween.Stop(gameObject);
    }

    public void ShakeFinished()
    {
        if(shakeCallback != null)
            shakeCallback();

        Debug.Log("Shake finished");
    }

    public void StartGlow()
    {
        glow.GetComponent<Animator>().SetTrigger("Glow");
    }

    public void StopGlow()
    {
        glow.GetComponent<Animator>().SetTrigger("FadeOut");
    }
}
