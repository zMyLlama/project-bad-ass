using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCollision : MonoBehaviour
{
    [SerializeField] Combat combat;

    private void OnTriggerEnter2D(Collider2D other) {
        combat.swordCollisionEvent(other);
    }
}
