using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class InputManager_Startscreen : MonoBehaviour 
{
    public CameraController cameraController;
    public StartscreenController startscreenController;
    public UI_Controller_StartScreen uiController;

    private RaycastHit2D raycastHit;
    public LayerMask fruitlayerMask;
    public LayerMask ravenlayerMask;

    private bool blocked = false;

    // --- Swipe Detection ---
    public bool allowSwipe = true;
    public bool allowShake = true;

    public EventSystem gameEventSystem;

    [Header("SFX")]
    public AudioClip threeShake;
    public AudioClip threePop;

    private void Start()
    {
        Initalize();
        Input.simulateMouseWithTouches = true;
    }

    public void Initalize()
    {
        
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    private void Update() 
    {
        if(startscreenController.startLoading)
        {
            return;
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(uiController.inPopupWindow)
            {
                Debug.Log("Hide Popups");
                uiController.HidePopupWindows();
            }
            else
            {
                Debug.Log("Quit the App");
                startscreenController.QuitApp();
            }
        }

        if(startscreenController.introIsPlaying && Input.GetMouseButtonUp(0))
        {
            startscreenController.SkipIntro();
        }

        if(Camera.main == null)
            return;
            
        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;

            if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 1000.0f, ravenlayerMask))
            {
                RavenController ravenController = hit.collider.transform.GetComponent<RavenController>();

                if(ravenController != null && ravenController.isIdling)
                {
                    Debug.Log("RavenPoke started");
                    ravenController.ScripedPoke(ravenController.Idle);
                }      
            }

            // --- Logo Tracking ---
            PointerEventData pointer = new PointerEventData(EventSystem.current);
            pointer.position = Input.mousePosition;

            List<RaycastResult> raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointer, raycastResults);

            if(raycastResults.Count > 0)
            {
                if(raycastResults[0].gameObject.CompareTag("Logo"))
                {
                    uiController.StartLogoAnimation();
                }
            }
        }
            
        if(blocked)
        {
            return; 
        }
     
        // -- Drag fruit --
        if(Input.GetMouseButtonDown(0))
        {
            raycastHit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition), 300.0f, fruitlayerMask);
                        
            if (raycastHit.collider != null)
            {
                if (raycastHit.transform.CompareTag("TouchableObject"))
                {
                    TouchResponse touchObject = raycastHit.transform.GetComponent<TouchResponse>();

                    if (touchObject != null)
                    {
                        //Debug.Log("TouchableObject started");
                        touchObject.StartParticleSystem(raycastHit.point);
                    }
                }
            }
            else
            {
                RaycastHit hit;

                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100.0f, fruitlayerMask))
                {
                    TouchResponse touchObject = hit.collider.transform.GetComponent<TouchResponse>();

                    if (touchObject != null)
                    {
                        //Debug.Log("TouchableObject started");
                        touchObject.StartParticleSystem(hit.point);
                    }
                } 
            }
        }

        // -- If Mouse Up --
        // Detect if you released at the basket, you are the right colour, or if you need to return to original position
        if(Input.GetMouseButtonUp(0))
        {
            
        }
    }

    private void OnEnable()
    {
        if(cameraController)
            cameraController.EnableMovement(true);
    }

    private void OnDisable()
    {
        if(cameraController)
            cameraController.EnableMovement(false);
    }

    public void BlockInput()
    {
        blocked = true;
    }

    public void UnblockInput()
    {
        blocked = false;
    }

    #region SFX

    private void PlayShakeSound(UnityAction callback)
    {
        AudioManager.Instance.SetSFXChannel(threeShake, callback, 0, 2);
    }

    private void PlayPopSound(UnityAction callback)
    {
        AudioManager.Instance.SetSFXChannel(threePop, callback, 0, 2);
    }

    #endregion
}