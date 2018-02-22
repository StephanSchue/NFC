using UnityEngine;
using System.Collections;

public class EarthWormController : MonoBehaviour 
{
    private SpriteRenderer spriteRenderer;
    public float time = 5.0f;
    public iTween.EaseType easeType;
    public Vector3 offset = Vector3.zero;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void StartAppear(FruitController fruitController)
    {
        transform.position = fruitController.transform.position + offset;

        spriteRenderer.sortingLayerName = fruitController.spriteRender.sortingLayerName;
        spriteRenderer.sortingOrder = fruitController.spriteRender.sortingOrder-1;
        spriteRenderer.enabled = true;

        /*
        iTween.MoveTo(gameObject, iTween.Hash(
            "position", (transform.position + offset) + (Vector3.right * 0.2f * (Random.Range(0, 2) == 0 ? 1 : -1)),
            "time", time,
            "delay", 0.0f,
            "easetype", easeType,
            "onComplete", "AppearComplete",
            "onCompleteTarget", gameObject
        ));
        */
    }

    public void AppearComplete()
    {
        spriteRenderer.enabled = false;
    }
}
