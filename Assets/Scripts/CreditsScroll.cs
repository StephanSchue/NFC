using UnityEngine;
using System.Collections;

public class CreditsScroll : MonoBehaviour 
{
    public Camera camera;
    public Transform scrollMovePosition;
    public float time = 2.0f;
    public float delay = 0.0f;

    public iTween.EaseType easeType;

    public bool aspect16by9 = true;
    public bool aspect4by2 = true;
    public bool aspect3by2 = true;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

	// Use this for initialization
	public void StartScroll() 
    {
        if(aspect16by9 && camera.aspect >= 1.7)
        {
            // 16:9
            Scroll(scrollMovePosition, time, delay);
        }
        else if(aspect4by2 && camera.aspect >= 1.48)
        {
            // 3:2
            Scroll(scrollMovePosition, time, delay);
        }
        else if(aspect3by2)
        {
            // 4:3
            Scroll(scrollMovePosition, time, delay);
        }
	}

    private void Scroll(Transform destination, float time, float delay=0.0f)
    {
        iTween.MoveTo(gameObject, iTween.Hash(
            "position", destination.position,
            "time", time,
            "delay", delay,
            "easetype", easeType,
            "onComplete", "IsComplete",
            "onCompleteTarget", gameObject
        ));
    }

    public void ResetPosition()
    {
        transform.position = startPosition;
    }

    private void IsComplete()
    {

    }
}
