using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class SimpleRope : MonoBehaviour
{
	public GameObject RopePrefab;
	public int width = 5;

	private GameObject curRopeElement;
	private Rigidbody2D lastRopeElement;

	private Vector3 anchorPosition = Vector3.zero;

	// Use this for initialization
	public void CreateRobe() 
	{
		ClearRobe();

		lastRopeElement = gameObject.GetComponent<Rigidbody2D>();
		lastRopeElement.isKinematic = true;

		anchorPosition = Vector3.zero;

		for (int i = 0; i < width; i++)
		{
			curRopeElement = GameObject.Instantiate<GameObject>(RopePrefab);
			curRopeElement.transform.parent = transform;
			curRopeElement.transform.localPosition = anchorPosition * curRopeElement.transform.localScale.y;

			SpriteRenderer spriteRenderer = curRopeElement.GetComponent<SpriteRenderer>();

			if(lastRopeElement != null)
			{
				HingeJoint2D curJoint = curRopeElement.GetComponent<HingeJoint2D>();
				HingeJoint2D lastJoint = lastRopeElement.GetComponent<HingeJoint2D>();
				curJoint.connectedBody = lastRopeElement.GetComponent<Rigidbody2D>();

				if(i > 0)
				{
					//lastJoint.connectedAnchor = new Vector2(0, spriteRenderer.bounds.extents.y * 4);
					curJoint.connectedAnchor = new Vector2(0, -2.9f);
				}
			}

			lastRopeElement = curRopeElement.GetComponent<Rigidbody2D>();
			anchorPosition -= new Vector3(0, 2.9f, 0);
		}
	}

	public void ClearRobe()
	{
		foreach (Transform item in gameObject.GetComponentsInChildren<Transform>()) 
		{
			if(item.gameObject != gameObject)
				DestroyImmediate(item.gameObject);
		}
	}
}
