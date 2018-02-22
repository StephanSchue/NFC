using UnityEngine;
using System.Collections;

public class FlyingBee : MonoBehaviour
{
    public iTween.EaseType startupEaseType;
    public float startUpTime = 3.0f;

    public float stepFrom = 0.01f;
    public float stepTo = 0.03f;

    public float heightFrom = 0.01f;
    public float heightTo = 0.03f;

    private float speed;
    private float height;
    // Update is called once per frame

    private bool startupDone = false;
    public bool shuffleBeeFlyingSound = false;

    private AudioSource audioSource;
    public AudioClip[] sfx_flying;

    private bool faster = false;
    public float fasterTimeAmount = 2.0f;
    private float fasterTime = 0.0f;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartUp();
    }

    void FixedUpdate()
    {
        if(!startupDone)
            return;

        if(Camera.main.WorldToViewportPoint(transform.position).x < -0.1f)
        {
            Destroy(gameObject);
        }

        Flying();
    }

    public void PlayingFlyAudio()
    {
        audioSource.clip = sfx_flying[Random.Range(0, sfx_flying.Length)];
        audioSource.Play();

        if(shuffleBeeFlyingSound)
        {
            StartCoroutine(DoCallback(PlayingFlyAudio, audioSource.clip.length));
        }
    }

    public IEnumerator DoCallback(UnityAction callback, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (callback != null)
        {
            callback();
            callback = null;
        }
    }

    private void StartUp()
    {
        //PlayingFlyAudio();

        iTween.MoveTo(gameObject, iTween.Hash(
            "path", new Vector3[] { transform.position + (Vector3.up * Random.Range(1.0f, 3.0f)), 
            transform.position + (Vector3.up * Random.Range(1.5f, 3.5f)) + (Vector3.left * Random.Range(0.5f, 2.0f)), 
            transform.position + (Vector3.up * Random.Range(2.0f, 4.0f)), 
            transform.position + (Vector3.up * Random.Range(1.5f, 3.5f)) + (Vector3.right * Random.Range(0.5f, 2.0f)) },
            "time", startUpTime,
            "delay", 0.0f,
            "easetype", startupEaseType,
            "onComplete", "StartUpDone",
            "onCompleteTarget", gameObject
        ));
    }

    private void Flying()
    {
        if(faster && Time.time > fasterTime)
        {
            faster = false;
        }

        speed = Random.Range(stepFrom, stepTo) * (faster ? 2.0f : 1.0f);
        height = Random.Range(heightFrom, heightTo);

        iTween.MoveUpdate(gameObject, iTween.Hash(
            "position", new Vector3(transform.position.x + speed, transform.position.y + height, transform.position.z), 
            "time", 0.2f
        ));
    }

    private void StartUpDone()
    {
        startupDone = true;
    }

    public void Tapped()
    {
        faster = true;
        fasterTime = Time.time + fasterTimeAmount;

        audioSource.clip = sfx_flying[Random.Range(0, sfx_flying.Length)];
        audioSource.Play();
    }
}
