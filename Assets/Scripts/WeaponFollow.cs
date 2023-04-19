using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponFollow : MonoBehaviour
{
    public GameObject player;
    public float radius = 1.0f;
    private Rigidbody2D playerRigidbody;

    private void Start() {
        playerRigidbody = player.GetComponent<Rigidbody2D>();
    }

    Dictionary<Vector2, float> _velocityToRotation = new Dictionary<Vector2, float>()
    {
        {new Vector2(0f, 0f), 0f}, // Still
        {new Vector2(0f, -1f), 0f}, // Downwards
        {new Vector2(0f, 1f), 180f}, // Upwards
        {new Vector2(1f, 0f), 90f}, // Right
        {new Vector2(-1f, 0f), 270f}, // Left

        {new Vector2(0.7f, 0.7f), 135f}, // Upwards Right
        {new Vector2(-0.8f, 0.7f), 225f}, // Upwards Left
        {new Vector2(0.7f, -0.8f), 45f}, // Downwards Right
        {new Vector2(-0.8f, -0.8f), 315f}, // Downwards Left
    };

    Vector2 _lastNormalizedVelocity = new Vector2(0f, 0f);
    public void FixedUpdate() {
        Vector2 _velocityNormalized = new Vector2(playerRigidbody.velocity.x, playerRigidbody.velocity.y).normalized;
        _velocityNormalized = new Vector2(Mathf.Floor(_velocityNormalized.x * 10f) / 10f, Mathf.Floor(_velocityNormalized.y * 10f) / 10f);

        if (_lastNormalizedVelocity != _velocityNormalized) {
            print("Apply rotation");
            transform.DORotate(new Vector3(0, 0, _velocityToRotation[_velocityNormalized]), 0.2f);
        }
        _lastNormalizedVelocity = _velocityNormalized;
    }
}
