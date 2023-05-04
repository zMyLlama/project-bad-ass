using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class WizardMove : MonoBehaviour
{
    //Script til mage fjende.

    [Header("Settings")]
    public float maxHealth = 50.0f;
    public int Speed = 4;
    public float runSpeed = -0.3f;
    public int HP = 5;
    public float runRange = 6;
    [SerializeField] float timeBetweenChargeAndFireball = 1.5f;
    [Header("Hostility")]
    public Transform target;
    [Header("Objects")]
    public GameObject[] disableOnDeath = new GameObject[ ]{};
    public ShakeManager shakeManager;
    public SpriteRenderer mySprite;
    public GameObject fireballPrefab;
    public GameObject explosionParticle;
    public GameObject chargeParticle;
    public Image radialHealth;
    public SpriteRenderer rippleFullscreen; 


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

    IEnumerator shootFireball() {
        shakeManager.addShakeWithPriority(1, 1, timeBetweenChargeAndFireball, 1);
        chargeParticle.GetComponent<ParticleSystem>().Play();
        
        yield return new WaitForSeconds(timeBetweenChargeAndFireball);

        shakeManager.addShakeWithPriority(4, 2, 0.2f, 10);
        chargeParticle.GetComponent<ParticleSystem>().Stop();

        GameObject fireball = Instantiate(fireballPrefab, transform.position, Quaternion.identity);
        fireball.GetComponent<Fireball>().Player = target;
        CD = 0;
    }

    float CD = 0;
    int timeUntilNextAttack = 5;
    void Update()
    {
        if (dead) return;
        if (CD == -1) rb.velocity = new Vector2(0, 0);

        Vector3 direction = target.position - transform.position;
        direction.Normalize();
        if (CD != -1) rb.velocity = direction * Speed;

        if (runRange > Vector3.Distance(transform.position, target.position) && CD != -1)
            rb.velocity = direction / runSpeed;

        if (CD != -1) CD += Time.deltaTime;
        if(CD >= timeUntilNextAttack && CD != -1)
        {
            CD = -1;
            timeUntilNextAttack = Random.Range(5, 10);
            StartCoroutine("shootFireball");
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
            Destroy(gameObject, 1f);
            currentHealth = 0;

            rippleFullscreen.material.SetFloat("_WaveDistanceFromCenter", -0.1f);
            rippleFullscreen.material.DOFloat(1, "_WaveDistanceFromCenter", 1f).OnComplete(() => rippleFullscreen.material.SetFloat("_WaveDistanceFromCenter", -0.1f));

            shakeManager.addShakeWithPriority(6, 1, 0.2f, 2);
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            explosionParticle.GetComponent<ParticleSystem>().Play();
            for (int i = 0; i < disableOnDeath.Length; i++)
            {
                disableOnDeath[i].SetActive(false);
            }
        }

        radialHealth.fillAmount = Remap(currentHealth, 0f, maxHealth, 0f,  1f);
    }
}
