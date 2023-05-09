using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Movement : MonoBehaviour
{
    #region Public Variables
    [Header("Velocities")]
    public float xVelocity = 0f;
    public float yVelocity = 0f;
    public float dashDistance = 0.5f;

    [Header("Keybinds")]
    public List<KeyCode> movementKeybinds = new List<KeyCode>();
    public List<float> representingMovementVelocities = new List<float>();

    [Header("Other")]
    public ShakeManager shakeManager;
    public GameObject mySprite;
    public Animator playerAnimator;
    public Transform feetLocation;
    public GameObject dustCloud;
    public LayerMask dashDetectionLayer;
    #endregion

    #region Private Variables
    Rigidbody2D _rb;
    Dictionary<int, string> representingAnimationNames = new Dictionary<int, string>();
    [HideInInspector] public bool _isDashing = false;
    #endregion

    #region Methods
    private IEnumerator WalkLoop() {
        while (true)
        {
            GameObject SFX = Instantiate(Resources.Load("SFX/FootstepAudio_1", typeof(GameObject)), transform.position, Quaternion.identity) as GameObject;
            SFX.GetComponent<AudioSource>().pitch = Random.Range(1f, 2f);
            SFX.transform.SetParent(GameObject.FindGameObjectWithTag("Cleaner").gameObject.transform);
            Destroy(SFX, 1f);

            GameObject _dustCloudClone = Instantiate(dustCloud, feetLocation.position, Quaternion.identity);
            _dustCloudClone.transform.rotation = Quaternion.Euler(0, 0, Random.Range(-180f, 180f));
            _dustCloudClone.transform.localScale = new Vector3(Random.Range(4f, 6f), Random.Range(4, 6f), 1);
            Destroy(_dustCloudClone, 0.5f);

            yield return new WaitForSeconds(0.3f);
        }
    }

    private void makeSpriteClone(Vector2 pos) {
        GameObject _spriteClone = Instantiate(mySprite);
        Destroy(_spriteClone, 1f);
        _spriteClone.transform.SetParent(GameObject.FindGameObjectWithTag("Cleaner").transform, true);
        _spriteClone.transform.position = pos;

        _spriteClone.GetComponent<SpriteRenderer>().DOFade(0, 0.5f);
    }

    private void dashSFX() {
        GameObject SFX = Instantiate(Resources.Load("SFX/FootstepAudio_1", typeof(GameObject)), transform.position, Quaternion.identity) as GameObject;
        SFX.GetComponent<AudioSource>().pitch = Random.Range(1f, 2f);
        SFX.transform.SetParent(GameObject.FindGameObjectWithTag("Cleaner").gameObject.transform);
        Destroy(SFX, 1f);
    }

    private IEnumerator flashStep(Vector2 start, Vector2 end) {
        shakeManager.addShakeWithPriority(3, 2, 0.1f, 9);
        makeSpriteClone(Vector2.Lerp(start, end, 0));
        dashSFX();
        yield return new WaitForSeconds(0.15f / 3f);
        makeSpriteClone(Vector2.Lerp(start, end, 0.33f));
        dashSFX();
        yield return new WaitForSeconds(0.15f / 3f);
        makeSpriteClone(Vector2.Lerp(start, end, 0.66f));
        dashSFX();
    }

    Coroutine _dashEnableRoutine = null;
    private IEnumerator handleDashEnabled() {
        _isDashing = true;
        yield return new WaitForSeconds(0.15f);
        _isDashing = false;
    }
    #endregion

    #region Start
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();

        representingAnimationNames.Add(0, "WalkUp");
        representingAnimationNames.Add(1, "WalkSide");
        representingAnimationNames.Add(2, "WalkDown");
        representingAnimationNames.Add(3, "WalkSide");
    }
    #endregion

    #region Update
    KeyCode _lastPressedXVelocityKey;
    KeyCode _lastPressedYVelocityKey;
    IEnumerator _walkLoopCoroutine = null;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            Vector2 _currentRBVelocity = _rb.velocity;
            if (_currentRBVelocity == new Vector2(0, 0)) _currentRBVelocity = new Vector2(0, representingMovementVelocities[2]);

            RaycastHit2D hit = Physics2D.Raycast(transform.position, _currentRBVelocity, dashDistance, dashDetectionLayer);
            Debug.DrawRay(transform.position, _currentRBVelocity.normalized * dashDistance, Color.red, 2f);

            Vector2 _endDashPos = hit.point;
            if (hit.collider == null) _endDashPos = new Vector2(transform.position.x, transform.position.y) + (_currentRBVelocity.normalized * dashDistance);

            StartCoroutine(flashStep(gameObject.transform.position, _endDashPos));
            transform.DOMove(_endDashPos, 0.15f).SetId("Dash");
            
            if (_dashEnableRoutine != null) StopCoroutine(_dashEnableRoutine);
            _dashEnableRoutine = StartCoroutine(handleDashEnabled());
        }

        foreach (var key in movementKeybinds) {
            int _keybindLocationInList = movementKeybinds.FindIndex((x) => x == key);

            if (Input.GetKeyDown(key))
            {
                if (_walkLoopCoroutine == null) {
                    _walkLoopCoroutine = WalkLoop();
                    StartCoroutine(_walkLoopCoroutine);
                }

                if (_keybindLocationInList == 0 || _keybindLocationInList == 2) {
                    _lastPressedYVelocityKey = key;
                    yVelocity = representingMovementVelocities[_keybindLocationInList];
                } else {
                    playerAnimator.Play(representingAnimationNames[_keybindLocationInList]);
                    
                    _lastPressedXVelocityKey = key;
                    xVelocity = representingMovementVelocities[_keybindLocationInList];
                }
            }

            if (Input.GetKeyUp(key))
            {
                if (_keybindLocationInList == 0 || _keybindLocationInList == 2) {
                    if (_lastPressedYVelocityKey == key) yVelocity = 0; 
                } else {
                    if (_lastPressedXVelocityKey == key) xVelocity = 0; 
                }
            }
        }

        if (yVelocity == (representingMovementVelocities[0]) && xVelocity == 0) {
            playerAnimator.Play(representingAnimationNames[0]);
        } else if (yVelocity == (representingMovementVelocities[2]) && xVelocity == 0) {
            playerAnimator.Play(representingAnimationNames[2]);
        }

        if (_rb.velocity == new Vector2(0, 0) && _walkLoopCoroutine != null) {
            StopCoroutine(_walkLoopCoroutine);
            _walkLoopCoroutine = null;
        }

        _rb.velocity = new Vector2(xVelocity, yVelocity);
        if (xVelocity < 0) { transform.GetChild(1).GetComponent<SpriteRenderer>().flipX = true; } else if (xVelocity > 0) { transform.GetChild(1).GetComponent<SpriteRenderer>().flipX = false; };
        playerAnimator.SetFloat("velocityX", xVelocity);
        playerAnimator.SetFloat("velocityY", yVelocity);
    }
    #endregion
}
