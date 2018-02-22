using UnityEngine;
using System.Collections;

public class CloudMovement : MonoBehaviour 
{
    public bool playOnAwake = true;
    private bool startMovement = false;

    public Vector3[] movementPositions;

    public bool playMovementTimeRandom = true;
    public float playMovementRandomTimeMin = 0.5f;
    public float playMovementRandomTimeMax = 1.3f;

    private int movementIndex = 0;
    private float movementItemTime = 1.0f;

    private void Start()
    {
        if(playOnAwake)
            StartMovement();
    }

    private void OnDestroy()
    {
        StopMovement();
    }

    #region Rotation

    public void StartMovement()
    {
        movementIndex = 0;

        if(playMovementTimeRandom)
            movementItemTime = Random.Range(playMovementRandomTimeMin, playMovementRandomTimeMax);
        
        startMovement = true;

        DoMovement();
    }

    public void DoMovement()
    {
        if(!startMovement)
            return;

        iTween.MoveBy(gameObject, iTween.Hash(
            "x", movementPositions[movementIndex].x,
            "y", movementPositions[movementIndex].y,
            "z", movementPositions[movementIndex].z,
            "time", movementItemTime,
            "delay", 0.0f,
            "easetype", "easeInOutQuad",
            "onComplete", "DoMovement",
            "onCompleteTarget", gameObject
        ));

        ++movementIndex;

        if(movementIndex >= movementPositions.Length)
        {
            movementIndex = 0;
        }
    }

    public void StopMovement()
    {
        startMovement = false;
        iTween.Stop(gameObject);
    }

    #endregion
}
