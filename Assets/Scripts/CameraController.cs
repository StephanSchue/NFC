using UnityEngine;
using System.Collections;

public delegate void ScreenShakeFinished();

public class CameraController : MonoBehaviour 
{
    private Vector3 basePosition = Vector3.zero;
	private Vector3 destinationPosition;
	private Vector3 moveVector;

	private Vector3 learpPosition;

	private Vector3 direction;
	private bool back = false;

	private float time = 0.0f;
	private float k_TotalTime = 0.0f;

	public float clampMagnitude = 10f;
	public bool fixZPosition = true;
	public bool clampYMinus = true;

	float horizontal = 0.0f;
	float vertical = 0.0f;

	private Vector3 pressButtonPosition = Vector3.zero;
	public bool enableMovement = true;
	private bool isOverUIElement = false;

	private Vector3 lastQcceleration = Vector3.zero;

    private float dropBackMultiplayer = 3.0f;
    private bool moving = false;

    public float dropBackMultiplayerMax = 10.0f;

    public float horizontalClamp = 1.75f;

    private Vector3 baseAcceleration;
    private Vector3 newAcceleration;

    public float speed = 1.0f;
    private ScreenShakeFinished screenShakeFinishedCallback;


	/// <summary>
	/// Use this for initialization
	/// </summary>
	void Start() 
	{
        baseAcceleration = Input.acceleration;
	}

	/// <summary>
	/// Update is called once per frame
	/// </summary>
	void Update () 
	{
        newAcceleration = Input.acceleration - baseAcceleration;
        horizontal = Mathf.Clamp(newAcceleration.x, -1.0f, 1.0f);

        //Debug.Log(baseAcceleration + " - " + Input.acceleration + " = " + newAcceleration);

        if(!enabled)
            return;

        //moving = false;

        // --- keyboard axis ---
        /*
        if(Input.GetAxis("Horizontal") != 0.0f)
        {
            horizontal += Mathf.Clamp(Input.GetAxis("Horizontal"), -0.1f, 0.1f);
            moving = true;
        }

        if(Input.GetAxis("Vertical") != 0.0f)
        {
            vertical += Mathf.Clamp(Input.GetAxis("Vertical"), 0.0f, 0.5f);
            moving = true;
        }*/
		
        /*
        if(Mathf.Abs(newAcceleration.z) > 0.1f) 
		{
            vertical = Mathf.Clamp(newAcceleration.z, -0.1f, 0.1f);
            moving = true;
		}
        */      

        /*
        if(!moving)
        {
            //horizontal = Mathf.Clamp(horizontal, -3.0f, 3.0f);
            vertical = Mathf.Clamp(vertical, -2.0f, 2.0f);
            back = true;
        }*/

        //lastQcceleration = Input.acceleration;

        // --- acceleration ---
        if(destinationPosition != Vector3.zero)
        {
            transform.position = Vector3.Lerp(transform.position, destinationPosition, Time.deltaTime * speed);

            if(Vector3.Distance(transform.position, destinationPosition) <= 0.1f)
            {
                destinationPosition = Vector3.zero;
                moving = false;
            }
        }

		if(horizontal != 0.0f) 
		{
            if(basePosition == Vector3.zero)
            {
                basePosition = transform.position;
            }

			moveVector = new Vector3(horizontal, vertical, 0.0f);
			MoveCamera(moveVector);
           
            /*
            if(Vector3.Distance(transform.position, basePosition) <= 0.1f)
            {
                horizontal = vertical = 0.0f;
                dropBackMultiplayer = 3.0f;
                back = false;
            }
            */
		}

        /*
		if(back)
		{
            if(horizontal != 0.0f)
                horizontal += (horizontal > 0 ? -Time.deltaTime : Time.deltaTime) * dropBackMultiplayer;

            if(vertical != 0.0f)
                vertical += (vertical > 0 ? -Time.deltaTime : Time.deltaTime) * dropBackMultiplayer;

            vertical = Mathf.Clamp(vertical, 0.0f, 100.0f);
            dropBackMultiplayer = dropBackMultiplayer < dropBackMultiplayerMax ? dropBackMultiplayer + 0.1f : dropBackMultiplayerMax;
		}
        */      
	}

	/// <summary>
	/// Moves the camera.
	/// </summary>
	/// <param name="vector">Vector.</param>
	private void MoveCamera(Vector3 vector)
	{
        destinationPosition = basePosition + vector; //Vector3.Lerp(transform.position, ): //Vector3.ClampMagnitude(transform.position + (Vector3.right * moveVector.x));
        destinationPosition = new Vector3(Mathf.Clamp(destinationPosition.x, -horizontalClamp, horizontalClamp), destinationPosition.y, destinationPosition.z);

        /*
		if(clampYMinus && destinationPosition.y <= basePosition.y) 
		{
			destinationPosition.y = basePosition.y;
		}
        */

        //transform.position = new Vector3(Mathf.Clamp(destinationPosition.x, -horizontalClamp, horizontalClamp), Mathf.Clamp((basePosition.y + vector.y), -10.0f, 5.0f), fixZPosition ? basePosition.z : destinationPosition.z);
	}

	public void EnableMovement(bool status)
	{
		enableMovement = status;
	}

    public void Shake(Vector3 strength, float time, ScreenShakeFinished callback=null, float delay=0.0f)
    {
        screenShakeFinishedCallback = callback;

        iTween.ShakeRotation(gameObject, iTween.Hash(
                "amount", strength,
                "time", time,
                "delay", delay,
                "onComplete", "ShakeFinished",
                "onCompleteTarget", gameObject
            ));
    }

    public void ShakeFinished()
    {
        if(screenShakeFinishedCallback != null)
        {
            screenShakeFinishedCallback();
        }
    }
}

/*
    if(UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
    {
        isOverUIElement = true;
        pressButtonPosition = Vector3.zero;
        return;
    }

    if(isOverUIElement)
    {
        if(Input.GetMouseButtonUp(0)) 
        {
            isOverUIElement = false;
        }

        return;
    }

    if (enableMovement) 
    {
        //horizontal = Input.GetAxis("Horizontal");
        //vertical = Input.GetAxis("Vertical");

        if (horizontal != 0.0f || vertical != 0.0f) {
            moveVector = new Vector3 (horizontal, vertical, 0.0f);
            MoveCamera (moveVector);
        }

        // Touch
        if (Input.GetMouseButtonDown(0)) {
            pressButtonPosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(0)) {
            // Gets a vector that points from the player's position to the target's.
            Vector3 heading = Input.mousePosition - pressButtonPosition; //new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0.0f);

            if (heading == Vector3.zero)
                return;

            float distance = heading.magnitude;
            direction = heading / distance; // This is now the normalized direction.

            horizontal = heading.x / distance;
            vertical = heading.y / distance;
        }

        if (Input.GetMouseButtonUp(0)) {
            back = true;
        } 
    }
*/
