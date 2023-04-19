using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponFollow : MonoBehaviour
{
    public GameObject player;
    public Combat combatScript;
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
    float _deltaTimeOnLastRotation = Time.time;
    public void FixedUpdate() {
        Vector2 _velocityNormalized = new Vector2(playerRigidbody.velocity.x, playerRigidbody.velocity.y).normalized;
        _velocityNormalized = new Vector2(Mathf.Floor(_velocityNormalized.x * 10f) / 10f, Mathf.Floor(_velocityNormalized.y * 10f) / 10f);

        if (_lastNormalizedVelocity != _velocityNormalized) {
            float _timeSinceLastAttack = Time.time - _deltaTimeOnLastRotation;
            float _currentAttackSpeed = combatScript.currentSwordAttackSpeed;

            if (_timeSinceLastAttack < 0.15) {
                if (_timeSinceLastAttack <= 0.08) combatScript.currentSwordAttackSpeed = Mathf.Clamp(_currentAttackSpeed += 0.1f, combatScript.fastestAttackSpeed, combatScript.slowestAttackSpeed);
                if (_timeSinceLastAttack > 0.08) combatScript.currentSwordAttackSpeed = Mathf.Clamp(_currentAttackSpeed += 0.05f, combatScript.fastestAttackSpeed, combatScript.slowestAttackSpeed);
            } else {
                if (_timeSinceLastAttack <= 1 && _timeSinceLastAttack > 0.4) combatScript.currentSwordAttackSpeed = Mathf.Clamp(_currentAttackSpeed -= 0.5f, combatScript.fastestAttackSpeed, combatScript.slowestAttackSpeed);
                if (_timeSinceLastAttack <= 0.4) combatScript.currentSwordAttackSpeed = Mathf.Clamp(_currentAttackSpeed -= 0.2f, combatScript.fastestAttackSpeed, combatScript.slowestAttackSpeed);
                if (_timeSinceLastAttack > 1) combatScript.currentSwordAttackSpeed = combatScript.fastestAttackSpeed;
            }

            transform.DORotate(new Vector3(0, 0, _velocityToRotation[_velocityNormalized]), combatScript.currentSwordAttackSpeed);
            _deltaTimeOnLastRotation = Time.time;
        }
        _lastNormalizedVelocity = _velocityNormalized;
    }
}
