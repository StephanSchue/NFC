using UnityEngine;
using System.Collections;

public delegate void HelicopterCompleteAnimationCallback();
public delegate void HelicopterCompleteMovementCallback();
public delegate void HelicopterCompleteFallingCallback();

public class HelicopterController : MonoBehaviour 
{
    private Quaternion baseRotation;
    private Animator animator;
    private Rigidbody rigidbody;
    private SphereCollider collider;

    private HelicopterCompleteAnimationCallback currentAnimationCallback;
    private HelicopterCompleteMovementCallback currentMovementCallback;
    private HelicopterCompleteFallingCallback currentFallingCallback;

    private bool flying = false;

    #region basic methods

	protected void OnEnable()
	{
        baseRotation = transform.rotation;

        animator = GetComponentInChildren<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<SphereCollider>();
	}

	protected void OnDisable()
	{

	}

    #endregion

    #region flying methods

    /// <summary>
    /// Flies to.
    /// </summary>
    /// <param name="destination">Destination.</param>
    /// <param name="callback">Callback.</param>
    /// <param name="time">Time.</param>
    /// <param name="delay">Delay.</param>
    public void FlyTo(Transform[] path, HelicopterCompleteMovementCallback callback, float time=0.2f, float delay = 0.0f)
    {
        MoveToDestination(gameObject, path, callback, time, delay);
    }

    /// <summary>
    /// Move Transform to destination.
    /// </summary>
    private void MoveToDestination(GameObject movedObject, Transform[] path, HelicopterCompleteMovementCallback callback, float time=0.2f, float delay = 0.0f)
    {
        StartEngine();
        flying = true;

        iTween.MoveTo(movedObject, iTween.Hash(
            "path", path,
            "time", time,
            "delay", delay,
            "easetype", "easeInOutQuad",
            "onComplete", "MoveToDestinationComplete",
            "onCompleteTarget", gameObject
        ));

        currentMovementCallback = callback;
    }

    private void MoveToDestinationComplete()
    {
        flying = false;

        if (currentMovementCallback != null) 
        {
            currentMovementCallback();
        }
    }
        
    public void StartHelicopterCrash(Transform destination, HelicopterCompleteMovementCallback callback=null)
    {
        collider.enabled = false;
        MoveToDestination(gameObject, new Transform[] { transform, destination }, callback, 0.5f, 0.0f);
    }

    /// <summary>
    /// Let the helicopter falling down epic
    /// </summary>
    public void BlackHarkDown(HelicopterCompleteFallingCallback callback=null)
    {
        StopEngine();
        iTween.Stop(gameObject);

        rigidbody.isKinematic = false;
        rigidbody.AddForceAtPosition(
            (Vector3.up * (Random.Range(0, 2) == 1 ? 1 : -1) +
                Vector3.right * (Random.Range(0, 2) == 1 ? 1 : -1) * 2.0f +
                Vector3.forward * (Random.Range(0, 2) == 1 ? 1 : -1))
            * 200.0f, Vector3.up); //AddForce(Vector3.right * (Random.Range(0, 2) == 1 ? 1 : -1) * 10.0f + Vector3.up * 100f);
        currentFallingCallback = callback;

        StartCoroutine(DoFunctionWithDelay(ResetHelicopter, 2.0f));
    }

    private void ResetHelicopter()
    {
        rigidbody.isKinematic = true;
        collider.enabled = true;

        transform.rotation = baseRotation;

        enabled = false;
        gameObject.SetActive(false);

        if(currentFallingCallback != null)
        { 
            currentFallingCallback();
        }
    }

    private IEnumerator DoFunctionWithDelay(FruitHighlightCallback method, float delay)
    {
        yield return new WaitForSeconds(delay);
        method();
    }

    #endregion

    #region animator methods

    /// <summary>
    /// Starts the flying animation.
    /// </summary>
    public void StartEngine(HelicopterCompleteAnimationCallback callback = null)
    {
        animator.SetTrigger("StartEngine");
        currentAnimationCallback = callback;
    }

    /// <summary>
    /// Stops the flying animation.
    /// </summary>
    public void StopEngine(HelicopterCompleteAnimationCallback callback = null)
    {
        animator.SetTrigger("StopEngine");
        currentAnimationCallback = callback;
    }

    /// <summary>
    /// Animations the complete.
    /// </summary>
    public void AnimationComplete()
    {
        if (currentAnimationCallback != null) 
        {
            currentAnimationCallback();
        }
    }

    #endregion
}
