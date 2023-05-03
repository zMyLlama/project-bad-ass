using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WizardMove : MonoBehaviour
{
    //Script til mage fjende.

    [Header("Variables")]
    public float maxHealth = 50.0f;
    public int Speed = 4;
    public float runSpeed = -0.3f;
    public int HP = 5;
    public float runRange = 6;
    [Header("Hostility")]
    public Transform target;
    [Header("Objects")]
    public GameObject[] disableOnDeath = new GameObject[ ]{};
    public SpriteRenderer mySprite;
    public GameObject fireballPrefab;
    public GameObject explosionParticle;
    public Image radialHealth;


    Rigidbody2D rb;
    Color originalColor;
    float currentHealth = 0.0f;
    bool dead = false;

    float Remap(float value, float from1, float to1, float from2, float to2) {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        originalColor = mySprite.color;
    }

    float CD = 0;
    void Update()
    {
        if (dead) return;

        Vector3 direction = target.position - transform.position;
        direction.Normalize();
        rb.velocity = direction * Speed;
        // Det her er til at bev�ge sig imod spilleren

        if (runRange > Vector3.Distance(transform.position, target.position))
        {
            rb.velocity = direction / runSpeed;
            // Det her er s� at magen flygter/l�ber fra spilleren hvis den kommer for t�t p�.
        }

        CD += Time.deltaTime;
        if(CD >= 5)
        {
            GameObject fireball = Instantiate(fireballPrefab, transform.position, Quaternion.identity);
            fireball.GetComponent<Fireball>().Player = target;
            CD = 0;
            // Det her er til at magen skyder  en ildkule imod spilleren med et interval i sekunder.
        }
    }

    IEnumerator flashWhite() {
        mySprite.color = Color.white;
        yield return new WaitForSeconds(0.2f);
        mySprite.color = originalColor;
    }

    public void takeDamage(float amount) {
        if (dead) return;

        currentHealth -= amount;
        StartCoroutine("flashWhite");
        if (currentHealth <= 0) {
            dead = true;
            currentHealth = 0;

            explosionParticle.GetComponent<ParticleSystem>().Play();
            for (int i = 0; i < disableOnDeath.Length; i++)
            {
                disableOnDeath[i].SetActive(false);
            }
        }

        radialHealth.fillAmount = Remap(currentHealth, 0f, maxHealth, 0f,  1f);
    }
}
