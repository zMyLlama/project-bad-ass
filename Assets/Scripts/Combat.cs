using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat : MonoBehaviour
{
    public int hearts = 5;
    public float amountOfHeartStates = 5f;
    [HideInInspector] public float health = 0;

    public Animator weaponAnimator;
    [Header("Attack stats")]
    [Range(0.2f, 5f)] public float slowestAttackSpeed = 2f;
    [Range(0.2f, 5f)] public float fastestAttackSpeed = 0.2f;
    [Range(0.2f, 5f)] public float currentSwordAttackSpeed = 0.2f;

    private void Start() {
        health = hearts * amountOfHeartStates;

        Debug.Log("Full hearts: " + Mathf.Floor(health / amountOfHeartStates));
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            damagePlayer(1);
        }
    }

    public void damagePlayer(int amount) {
        health--;

        Debug.Log("Full hearts: " + Mathf.Floor(health / amountOfHeartStates));
        if (health % amountOfHeartStates != 0.0f) {Debug.Log("Missing heart state: " + health % amountOfHeartStates);} else {Debug.Log("No missing heart state");}
        Debug.Log("Empty hearts: " + (hearts - Mathf.Ceil(health / amountOfHeartStates)));

        Debug.Log("------------------------");
    }

    public void swordCollisionEvent() {

    }
}
