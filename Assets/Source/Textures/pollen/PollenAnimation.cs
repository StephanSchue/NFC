using UnityEngine;
using System.Collections;

public class PollenAnimation : MonoBehaviour
{
	public Rigidbody2D poll;
	public Animator animator;

    public EnvironmentController environmentController;

	void Start ()
	{
		StartCoroutine (AddForce ());
		if (Random.value > 0.5f) {
			transform.localScale = new Vector3 (-1, 1, 1);
		}
	}

	IEnumerator AddForce ()
	{
		while (true) {
            
			poll.AddForce (new Vector2 (Random.Range (-0.3f, 0.3f), Random.Range (0.1f, 1.3f)));
			animator.speed = Random.Range (0.8f, 1.2f);

			if (transform.position.x < -35 || transform.position.x > 35 || transform.position.y < -10 || transform.position.y > 30) 
            {
                environmentController.DecreasePolle();
				Destroy(gameObject);
				yield break;
			}
			yield return new WaitForSeconds (Random.Range (3, 7));
		}
	}

   
	
	
}
