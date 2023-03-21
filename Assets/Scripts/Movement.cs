using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    #region Public Variables
    [Header("Velocities")]
    public float xVelocity = 0f;
    public float yVelocity = 0f;

    [Header("Keybinds")]
    public List<KeyCode> movementKeybinds = new List<KeyCode>();
    public List<float> representingMovementVelocities = new List<float>();

    [Header("Other")]
    public Animator playerAnimator;
    public Transform feetLocation;
    public GameObject dustCloud; 
    #endregion

    #region Private Variables
    [Header("Serialized")]
    [SerializeField] Rigidbody2D _rb;
    #endregion

    #region Methods
    private IEnumerator walkLoop() {
        while (true)
        {
            playerAnimator.Play("Walk");

            GameObject _dustCloudClone = Instantiate(dustCloud, feetLocation.position, Quaternion.identity);
            _dustCloudClone.transform.rotation = Quaternion.Euler(0, 0, Random.Range(-180f, 180f));
            _dustCloudClone.transform.localScale = new Vector3(Random.Range(4f, 6f), Random.Range(4, 6f), 1);
            Destroy(_dustCloudClone, 0.5f);
            yield return new WaitForSeconds(0.3f);
        }
    }
    #endregion

    #region Start
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }
    #endregion

    #region Update
    KeyCode _lastPressedXVelocityKey;
    KeyCode _lastPressedYVelocityKey;
    IEnumerator _walkLoopCoroutine = null;
    void Update()
    {
        foreach (var key in movementKeybinds) {
            int _keybindLocationInList = movementKeybinds.FindIndex((x) => x == key);

            if (Input.GetKeyDown(key))
            {
                if (_walkLoopCoroutine == null) {
                    _walkLoopCoroutine = walkLoop();
                    StartCoroutine(_walkLoopCoroutine);
                }

                if (_keybindLocationInList == 0 || _keybindLocationInList == 2) {
                    _lastPressedYVelocityKey = key;
                    yVelocity = representingMovementVelocities[_keybindLocationInList];
                } else {
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

        if (_rb.velocity == new Vector2(0, 0) && _walkLoopCoroutine != null) {
            StopCoroutine(_walkLoopCoroutine);
            _walkLoopCoroutine = null;
        }

        _rb.velocity = new Vector2(xVelocity, yVelocity);
    }
    #endregion
}
