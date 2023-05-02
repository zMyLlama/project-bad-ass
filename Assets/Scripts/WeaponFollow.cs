using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponFollow : MonoBehaviour
{
    public GameObject player;
    public Image staminaFill;
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

    float Remap(float value, float from1, float to1, float from2, float to2) {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    Vector2 _lastNormalizedVelocity = new Vector2(0f, 0f);
    float _deltaTimeOnLastRotation = 0;
    public void FixedUpdate() {
        float _currentAttackSpeed = combatScript.currentSwordAttackSpeed;
        float _timeSinceLastAttack = Time.time - _deltaTimeOnLastRotation;
        if (_timeSinceLastAttack > 0.35) {
            combatScript.currentSwordAttackSpeed = Mathf.Clamp(_currentAttackSpeed - 0.05f, combatScript.fastestAttackSpeed, combatScript.slowestAttackSpeed);
            if (_timeSinceLastAttack > 1) combatScript.currentSwordAttackSpeed = combatScript.fastestAttackSpeed;
        } else if (_timeSinceLastAttack > 0.1) {
            combatScript.currentSwordAttackSpeed = Mathf.Clamp(_currentAttackSpeed - 0.015f, combatScript.fastestAttackSpeed, combatScript.slowestAttackSpeed);
        }

        Vector2 _velocityNormalized = new Vector2(playerRigidbody.velocity.x, playerRigidbody.velocity.y).normalized;
        _velocityNormalized = new Vector2(Mathf.Floor(_velocityNormalized.x * 10f) / 10f, Mathf.Floor(_velocityNormalized.y * 10f) / 10f);

        if (_lastNormalizedVelocity != _velocityNormalized) {
            if (_timeSinceLastAttack < 0.2) {
                if (_timeSinceLastAttack <= 0.08) combatScript.currentSwordAttackSpeed = Mathf.Clamp(_currentAttackSpeed + 0.05f, combatScript.fastestAttackSpeed, combatScript.slowestAttackSpeed);
                if (_timeSinceLastAttack > 0.08) combatScript.currentSwordAttackSpeed = Mathf.Clamp(_currentAttackSpeed + 0.025f, combatScript.fastestAttackSpeed, combatScript.slowestAttackSpeed);
            }

            transform.DORotate(new Vector3(0, 0, _velocityToRotation[_velocityNormalized]), combatScript.currentSwordAttackSpeed);
            _deltaTimeOnLastRotation = Time.time;
        }
        staminaFill.fillAmount = Remap(combatScript.currentSwordAttackSpeed, combatScript.slowestAttackSpeed, combatScript.fastestAttackSpeed, 0, 1);
        _lastNormalizedVelocity = _velocityNormalized;
    }
}
