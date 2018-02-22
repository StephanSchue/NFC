using UnityEngine;
using System.Collections;

public class CutTheRopeController : MonoBehaviour 
{
	public LayerMask layerMask;
	public int cutCount { get; private set; }

	private Rigidbody2D rigidBody2D;
	public CircleCollider2D myCollider { get; private set; }

	private HingeJoint2D cutedHinge;
	private BoxCollider2D cutedBoxCollider;

	/// <summary>
	/// Setup the Controller
	/// </summary>
	private void Start()
	{
		rigidBody2D = GetComponent<Rigidbody2D>();
		myCollider = GetComponent<CircleCollider2D>();
		Initalize();
	}

	public void ResetPosition()
	{
		transform.localPosition = new Vector3 (-5f, 8f, 0f);
	}

	/// <summary>
	///  Initialize the Controller
	/// </summary>
	private void OnEnabled()
	{
		Initalize();
	}

	/// <summary>
	/// Initalize all variables.
	/// </summary>
	public void Initalize()
	{
		cutCount = 0;
	}

	/// <summary>
	/// Activate the Collider and Rigidbody and move it by the MousePosition
	/// </summary>
	public void Cut()
	{
		rigidBody2D.MovePosition(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10)));

		/*
		//RaycastHit2D hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition), 200f, layerMask);

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		//RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 50.0f);
		RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10)), Vector2.zero);

		if (hit.collider != null) 
		{
			hit.collider.GetComponent<HingeJoint2D>().enabled = false;
		}
		*/
	}

	/// <summary>
	/// Raises the collision enter 2d event.
	/// </summary>
	/// <param name="coll">Coll.</param>
	private void OnCollisionEnter2D(Collision2D coll)
	{
		coll.gameObject.SetActive(false);

		//Destroy(coll.gameObject); 
		//cutedHinge = coll.gameObject.GetComponent<HingeJoint2D>();
		//cutedHinge.connectedBody = null;
		//cutedHinge.enabled = false;

		// disable so the user has to swipe a new one
		//collider.enabled = false;

		++cutCount;
	}
}
