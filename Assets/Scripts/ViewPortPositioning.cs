using UnityEngine;
using System.Collections;

public class ViewPortPositioning : MonoBehaviour 
{
    public Vector3 screenPoint;
    private Vector3 lastPosition;

	// Use this for initialization
	void Update() 
    {
        //if (lastPosition == transform.position)
        //    return;
        
        transform.position = Camera.main.ViewportToWorldPoint(screenPoint);
        lastPosition = transform.position;
	}
}
