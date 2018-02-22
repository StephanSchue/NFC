using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.Events;


public class InputManager : MonoBehaviour
{
    public GameController gameController;
    public BasketController basketController;
    public CameraController cameraController;
    public EnvironmentController environmentController;

    public LayerMask basketLayermask;

    private RaycastHit2D raycastHit;
    private FruitController _curFruit;
    private int spriteOrder = 0;

    private FruitController _curResetFruit;
    private bool resetFruit = false;
    private int fruitCount = 0;
    private int sortingOrderDirection = 1;

    private bool blocked = false;

    // --- Swipe Detection ---
    public bool allowSwipe = true;
    public bool allowShake = true;

    private Vector3 startSwipePosition;
    private float startSwipeTime;

    private bool isSwiping = false;
    private bool swiped = false;
    private bool blockSwiping = false;

    public Vector3 grabOffset = Vector3.zero;
    private bool startSwipeRight = false;
    private bool endSwipeRight = false;

    public LayerMask fruitlayerMask;
    public LayerMask ravenlayerMask;

    public EventSystem gameEventSystem;

    public Vector3 basketShakeItensity = Vector3.one;
    public float basketShakeTime = 1.0f;

    [Header("SFX")]
    public AudioClip threeShake;
    public AudioClip threePop;
    public AudioClip addToBasket;

    public AudioClip[] wrongFruit;

    public ParticleSystem sparcleSystem;

    [HideInInspector()]
    public bool blockGrabing = true;
    public bool grapped = false;
    private Vector3 lastMousePosition;
    private bool buttonDown = false;

    private void Start()
    {
        Initalize();
        Input.simulateMouseWithTouches = true;
        lastMousePosition = Input.mousePosition;
    }

    public void Initalize()
    {
        spriteOrder = 0;
        fruitCount = 0;

        BlockDiceRole();
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            buttonDown = true;

            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100.0f, ravenlayerMask))
            {
                RavenController ravenController = hit.collider.transform.GetComponent<RavenController>();

                if (ravenController != null && ravenController.isIdling)
                {
                    Debug.Log("RavenPoke started");
                    ravenController.ScripedPokeGame(ravenController.Idle);
                }
                else if (LayerMask.LayerToName(hit.transform.gameObject.layer) == "Env")
                {
                    if (environmentController.IsNight)
                        environmentController.InitDayMode();
                    else
                        environmentController.InitNightMode();
                }
            }
        }

        lastMousePosition = Input.mousePosition;
        buttonDown = false;
    }

    public void StopBasketShaking()
    {
        basketController.StopGlow();
        basketController.StopShake();
    }

    /// <summary>
    /// Fruits the is complete.
    /// </summary>
    private void FruitIsComplete()
    {
        if (resetFruit)
            _curResetFruit.Leave();

        _curResetFruit = null;
        resetFruit = false;

        /*
		if(!gameController.GameIsFinished)
			gameController.DiceController.AllowToRoll(true);
        */
    }

    private void OnEnable()
    {
        if (cameraController)
            cameraController.EnableMovement(true);
    }

    private void OnDisable()
    {
        if (cameraController)
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

    public void StartSwipe(bool right)
    {
        if (blockSwiping || blocked || !allowSwipe)
            return;

        startSwipeRight = right;
        isSwiping = true;
        swiped = false;
        startSwipePosition = Input.mousePosition;

        gameController.DisablePointerAtColorWheelHint();
    }

    public void EndSwipe(bool right)
    {
        //Debug.Log("blockSwiping: " + blockSwiping + "|| blocked " + blocked + " || !isSwiping: " + !isSwiping + " || !allowSwipe: " + !allowSwipe);
        if (blockSwiping || blocked || !isSwiping || !allowSwipe)
            return;

        endSwipeRight = right;
        swiped = true;
        isSwiping = false;

        gameController.DisablePointerAtColorWheelHint();
    }

    public bool IsSwipePossilbe()
    {
        return (!blockSwiping && !blocked && allowSwipe);
    }

    public void BlockDiceRole()
    {
        blockSwiping = true;

    }

    public void UnblockDiceRole()
    {
        blockSwiping = false;
    }

    public void RoleDice(bool directionRight = true)
    {
        if (_curFruit != null)
            return;

        gameController.RoleDice(directionRight);
    }

    #region SFX

    public void GrapFruit()
    {
        PlayPopSound(PlayShakeSound);
    }

    private void PlayShakeSound()
    {
        AudioManager.Instance.SetSFXChannel(threeShake, null, 0, 2);
    }

    private void PlayPopSound(UnityAction callback)
    {
        AudioManager.Instance.SetSFXChannel(threePop, callback, 0, 2);
        grapped = true;
    }

    #endregion

    public void ShotTreeSparkle(Vector3 position)
    {
        StartParticleSparcle(5.0f, position);
    }

    public void StartParticleSparcle(float duration, Vector3 position)
    {
        ParticleSystem partSys = ParticleSystem.Instantiate(sparcleSystem, position, Quaternion.identity) as ParticleSystem;
        StartCoroutine(ParticleEffectRunning(partSys, duration));
        Destroy(partSys.gameObject, 10.0f);
    }

    private IEnumerator ParticleEffectRunning(ParticleSystem particleSystem, float duration)
    {
        particleSystem.Play();

        yield return new WaitForSeconds(duration);

        if (particleSystem != null)
            particleSystem.Stop();
    }
}
