using UnityEngine;
using System.Collections;

public class PlaceObject : MonoBehaviour 
{
	public Transform parentNode;
	public GameObject prefab;
	public Vector3 position = Vector3.zero;

	private void Awake()
	{
		GameObject newGameObject = Instantiate<GameObject>(prefab);
		newGameObject.transform.parent = parentNode;
		newGameObject.transform.localPosition = position;
		newGameObject.transform.localRotation = Quaternion.identity;
	}
}
