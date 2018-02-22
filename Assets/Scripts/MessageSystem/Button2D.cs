using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class Button2D : MonoBehaviour 
{
	public GameObject gameController;
	public GameObject uiController;

	private SpriteRenderer spriteRenderer;
    private bool blocked = false;
    private RaycastHit hit;

	private void OnEnable()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
	}
        
    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            blocked = EventSystem.current.IsPointerOverGameObject(0);

            if(blocked)
                return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(ray, out hit))
            {
                if(hit.collider != null && hit.collider.gameObject.CompareTag("StartSign"))
                {
                    StartGame();
                }
            }
        }
    }

	private void StartGame() 
	{
        if(!enabled || blocked || EventSystem.current.IsPointerOverGameObject() || SceneManager.GetSceneByName("LoadingScreen").isLoaded)
			return;

		ExecuteEvents.Execute<IStartscreenMessageTarget>(gameController, null, (x,y)=>x.PressStart());
		ExecuteEvents.Execute<IStartscreenMessageTarget>(uiController, null, (x,y)=>x.PressStart());
	}

	private void OnMouseEnter()
	{
        if(!enabled || blocked || EventSystem.current.IsPointerOverGameObject() || SceneManager.GetSceneByName("LoadingScreen").isLoaded)
            return;

		spriteRenderer.material.SetFloat("_Val", 1.1f);
	}

	private void OnMouseExit()
	{
        if(!enabled || blocked || EventSystem.current.IsPointerOverGameObject() || SceneManager.GetSceneByName("LoadingScreen").isLoaded)
            return;

		spriteRenderer.material.SetFloat("_Val", 1.0f);
	}
}
