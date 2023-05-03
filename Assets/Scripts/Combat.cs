using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Combat : MonoBehaviour
{
    [Header("Health")]
    public int hearts = 5;
    public float amountOfHeartStates = 4f;

    public GameObject heartsHolder;
    public GameObject heart;
    public Sprite[ ] heartStates = new Sprite[  ]{};
    [HideInInspector] public float health = 0;


    public Animator weaponAnimator;
    [Header("Attack stats")]
    [Range(0.2f, 5f)] public float slowestAttackSpeed = 2f;
    [Range(0.2f, 5f)] public float fastestAttackSpeed = 0.2f;
    [Range(0.2f, 5f)] public float currentSwordAttackSpeed = 0.2f;
    public float baseDamage = 12.5f;
    public float attackCooldown = 0.15f;

    private void Start() {
        health = hearts * amountOfHeartStates;

        Debug.Log("Full hearts: " + Mathf.Floor(health / amountOfHeartStates));
        damagePlayer(0);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            damagePlayer(1);
        }
    }

    public void damagePlayer(int amount) {
        health--;
        for (int i = 0; i < heartsHolder.transform.childCount; i++)
        {
            Destroy(heartsHolder.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < Mathf.Floor(health / amountOfHeartStates); i++)
        {
            GameObject _heartClone = Instantiate(heart);
            _heartClone.transform.SetParent(heartsHolder.transform);
        }
        if (health % amountOfHeartStates != 0.0f) {
            GameObject _heartClone = Instantiate(heart);
            _heartClone.GetComponent<Image>().sprite = heartStates[Mathf.RoundToInt(health % amountOfHeartStates)];
            _heartClone.transform.SetParent(heartsHolder.transform);
        }
        for (int i = 0; i < (hearts - Mathf.Ceil(health / amountOfHeartStates)); i++)
        {
            GameObject _heartClone = Instantiate(heart);
            _heartClone.GetComponent<Image>().sprite = heartStates[0];
            _heartClone.transform.SetParent(heartsHolder.transform);
        }

        Debug.Log("Full hearts: " + Mathf.Floor(health / amountOfHeartStates));
        if (health % amountOfHeartStates != 0.0f) {Debug.Log("Missing heart state: " + health % amountOfHeartStates);} else {Debug.Log("No missing heart state");}
        Debug.Log("Empty hearts: " + (hearts - Mathf.Ceil(health / amountOfHeartStates)));

        Debug.Log("------------------------");
    }

    public void swordCollisionEvent(Collider2D other) {
        other.gameObject.GetComponent<WizardMove>().takeDamage(baseDamage);
    }
}
