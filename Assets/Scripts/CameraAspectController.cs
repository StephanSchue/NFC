using UnityEngine;
using System.Collections;

public class CameraAspectController : MonoBehaviour 
{
    public Vector3 aspect4to3 = Vector3.zero;
    public Vector3 aspect3to2 = Vector3.zero;
	public Vector3 aspect16to9 = Vector3.zero;

	// Use this for initialization
	void Start () 
    {
        //defaultAspect = transform.position;

        if(Camera.main.aspect >= 1.7)
        {
			transform.position = aspect16to9;
        }
        else if(Camera.main.aspect >= 1.48)
        {
            transform.position = aspect3to2;
        }
        else
        {
            transform.position = aspect4to3;
        }
	}
}
