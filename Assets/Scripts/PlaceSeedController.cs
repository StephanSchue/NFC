using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public delegate void PlaceASeed(Vector3 position, SpriteRenderer parentSpriteRenderer);

public class PlaceSeedController : MonoBehaviour 
{
    public Vector3 offset = new Vector3(0.0f, 0.0f, 10.0f);
	private Camera cameraTransform;
	private RaycastHit hit;
	private Ray ray;
    private Image imageRenderer;

	public PlaceASeed OnPlaceASeed;
	private SeedType curSeed;

    private float bockTime = 0.0f;
    private bool blocked = false;

    public float droppingDuration = 3.0f;
    private float droppingEndTime = 0.0f;

    public float[] flowerHights;

    private bool onWayToBack = false;

    public RewardGardenController rewardGardenController;

    public Transform[] seedBackPath;
    public float seedBackTime = 2.0f;
    public Vector3 seedBackOffset = Vector3.zero;
    public iTween.EaseType seedBackEaseType;

	// Use this for initialization
	void Start() 
	{
		cameraTransform = Camera.main;
	}
	
	// Update is called once per frame
	void Update() 
	{
        if(!enabled || onWayToBack || OnPlaceASeed == null)
			return;

        /*
        if(EventSystem.current.IsPointerOverGameObject())
        {
            //bockTime = Time.time + 0.1f;
            //blocked = true;
            return;
        }
        */

        /*
        if (Time.time > bockTime)
        {
            //Debug.Log("Unblocked");
            blocked = false;
        }
        */

        if(blocked)
            return;
            
        if(Input.GetMouseButtonDown(0))
        {
            //droppingEndTime = Time.time + droppingDuration;
        }
        
        if(Input.GetMouseButtonUp(0)) // && Time.time > droppingEndTime)
        {
            if(!PlaceSeedAtCurrentMousePosition())
            {
                SeedBackInBag();
            }
        }

        transform.position = Input.mousePosition + offset;
	}

    public void SeedBackInBag()
    {
        transform.SetAsFirstSibling();

        iTween.MoveTo(gameObject, iTween.Hash(
            "position", seedBackPath[0].position + seedBackOffset,
            "time", seedBackTime,
            "delay", 0.0f,
            "easetype", seedBackEaseType,
            "onComplete", "SeedIsArrived",
            "onCompleteTarget", gameObject
        ));

        iTween.ScaleTo(gameObject, iTween.Hash(
            "x", 0.4f,
            "y", 0.4f,
            "z", 0.4f,
            "time", seedBackTime,
            "delay", (seedBackTime*0.25f),
            "easetype", "linear"
        )); 

        onWayToBack = true;
    }

    public bool PlaceSeedAtCurrentMousePosition()
    {
        ray = cameraTransform.ScreenPointToRay(Input.mousePosition);
        
        PointerEventData pointer = new PointerEventData(EventSystem.current);
        pointer.position = Input.mousePosition;

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, raycastResults);

        // --- Place Flower ---
        if(raycastResults.Count > 0)
        {
            for (int i = 0; i < raycastResults.Count; i++)
            {
                if(raycastResults[i].gameObject.transform.CompareTag("StartSign"))
                {
                    Debug.Log("StartSign Blocked");
                    return false;
                }
                else if(raycastResults[i].gameObject.layer == 5 || raycastResults[i].gameObject.layer == 18)
                {
                    Debug.Log("UI/Flower Blocked");
                    return false;
                }  
            }
        }
         
        if(Physics.Raycast(ray, out hit, 1000.0f))
        {
            Debug.Log("Ready for planting!");

            if (rewardGardenController.seedPlaces.Length > 0)
            {
                for (int i = 0; i < rewardGardenController.seedPlaces.Length; i++)
                {
                    if (rewardGardenController.seedPlaces[i] != null)
                    {
                        //Debug.Log(Vector3.Distance(hit.point, rewardGardenController.seedPlaces[i].transform.position));

                        if (Vector3.Distance(hit.point, rewardGardenController.seedPlaces[i].transform.position) < 8.0f)
                        {
                            Debug.Log("Seed is to near a flower");
                            return false;
                        }
                    }
                }
            }

            /*
            if(hit.collider.CompareTag("Raven"))
            {
                Debug.Log("Raven");
                return false;
            }
            */

            SpriteRenderer spriteRenderer = hit.collider.GetComponentInParent<SpriteRenderer>();
            string sortingLayer = "";

            if(spriteRenderer != null)
            {
                sortingLayer = spriteRenderer.sortingLayerName;
                Debug.Log("Dropped");
            }
            else
            {
                return false;
            }

            float flowerHight = 6.0f;

            switch (sortingLayer)
            {
                case "Grass1":
                    flowerHight = flowerHights[0];
                    break;
                case "Grass2":
                    flowerHight = flowerHights[1];
                    break;
                case "Grass3":
                    flowerHight = flowerHights[2];
                    break;
                case "Grass4":
                    flowerHight = flowerHights[3];
                    break;
                case "Grass5":
                    flowerHight = flowerHights[4];
                    break;
                default:
                    break;
            }

            OnPlaceASeed(new Vector3(hit.point.x, hit.collider.transform.position.y + flowerHight, hit.point.z), hit.collider.GetComponentInParent<SpriteRenderer>());

            return true;
        }

        return false;
    }

    public void SeedIsArrived()
    {
        onWayToBack = false;
        enabled = false;
        transform.SetAsLastSibling();

        iTween.Stop(gameObject);
    }

	protected void OnEnable()
	{
        transform.position = Input.mousePosition + offset;
        transform.localScale = Vector3.one;

        imageRenderer = GetComponent<Image>();
        
        if(imageRenderer != null)
            imageRenderer.enabled = true;
	}

	protected void OnDisable()
	{
        if(imageRenderer != null)
            imageRenderer.enabled = false;
	}

	public void SetSeed(SeedType type)
	{
		curSeed = type;
	}
}
