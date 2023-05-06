using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class EnemyController : MonoBehaviour
{
    [Header("Object Reference")]
    public EnemyStats_Settings stats;
    public UnityEvent primaryAttackEvent;
    [Header("Objects")]
    public GameObject[] disableOnDeath = new GameObject[ ]{};
    public SpriteRenderer mySprite;
    public GameObject enemyHUD;
    public Image radialHealth;
    [Header("Particles")]
    public GameObject explosionParticle;
    public GameObject chargeParticle;
    public GameObject hitParticle;

    [HideInInspector] public Transform _target;
    [HideInInspector] public SpriteRenderer _rippleFullscreen; 
    [HideInInspector] public ShakeManager _shakeManager;
    Animator _animator;
    Rigidbody2D rb;
    Color originalColor;
    float currentHealth = 0.0f;
    bool dead = false;

    float Remap(float value, float from1, float to1, float from2, float to2) {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    void Start()
    {
        timeUntilNextAttack = Random.Range(stats.minPrimaryAttackCooldown, stats.maxPrimaryAttackCooldown);
        rb = GetComponent<Rigidbody2D>();
        currentHealth = stats.maxHealth;
        originalColor = mySprite.color;
        _animator = GetComponent<Animator>();

        _target = GameObject.FindGameObjectWithTag("Player").transform;
        _shakeManager = GameObject.FindGameObjectWithTag("Cinemachine").GetComponent<ShakeManager>();
        _rippleFullscreen = GameObject.FindGameObjectWithTag("Cinemachine").transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>();
    }

    IEnumerator shootFireball() {
        _shakeManager.addShakeWithPriority(1, 1, stats.timeBetweenChargeAndFireball, 1);
        chargeParticle.GetComponent<ParticleSystem>().Play();
        
        yield return new WaitForSeconds(stats.timeBetweenChargeAndFireball);

        chargeParticle.GetComponent<ParticleSystem>().Stop();

        for (int i = 0; i < 15; i++)
        {
            _shakeManager.addShakeWithPriority(2, 1, 0.1f, 10);

            primaryAttackEvent.Invoke();
            GameObject fireball = Instantiate(stats.primaryProjectilePrefab, transform.position, Quaternion.identity);
            fireball.GetComponent<Fireball>().bulletSettings.target = _target.transform.position;

            yield return new WaitForSeconds(0.1f);
        }

        CD = 0;
    }

    float _knockbackTimer = 0;
    float _knockbackForce;
    Vector2 _knockbackDirection; 

    [HideInInspector] public float CD = 0;
    float timeUntilNextAttack = 5;
    void Update()
    {
        if (dead) return;
        if (CD == -1) rb.velocity = new Vector2(0, 0);

        Vector2 direction = (_target.position - transform.position).normalized;
        if (_knockbackTimer > 0)
        {
            rb.AddForce(_knockbackDirection * (_knockbackForce * 80) * Time.deltaTime, ForceMode2D.Impulse);
            _knockbackTimer -= Time.deltaTime;
        }

        if (CD != -1 && _knockbackTimer <= 0)
            rb.velocity = direction * stats.speed;

        if (3f > Vector3.Distance(transform.position, _target.position) && CD != -1 && _knockbackTimer <= 0 && stats.fightingStyle == FightingStyles.Normal)
            rb.velocity = (direction * stats.speed) * 0.5f;

        if (stats.fleeRange > Vector3.Distance(transform.position, _target.position) && CD != -1 && _knockbackTimer <= 0 && stats.fightingStyle == FightingStyles.Coward)
            rb.velocity = direction / stats.fleeSpeed;

        if (CD != -1) CD += Time.deltaTime;
        if(CD >= timeUntilNextAttack && CD != -1)
        {
            CD = -1;
            timeUntilNextAttack = Random.Range(stats.minPrimaryAttackCooldown, stats.maxPrimaryAttackCooldown);
            primaryAttackEvent.Invoke();
        }
    }

    IEnumerator flashWhite() {
        mySprite.color = Color.white;
        yield return new WaitForSeconds(0.2f);
        mySprite.color = originalColor;
    }

    public void applyKnockback(Vector2 direction, float force, float duration) {
        _knockbackDirection = direction;
        _knockbackForce = force;
        _knockbackTimer = duration;
    }

    public void takeDamage(float amount) {
        if (dead) return;
        float healthBeforeAppliedDamage = currentHealth;

        currentHealth -= amount;
        StartCoroutine("flashWhite");
        if (currentHealth <= 0) {
            dead = true;
            Destroy(gameObject, 1f);
            gameObject.transform.SetParent(GameObject.FindGameObjectWithTag("Cleaner").transform);
            currentHealth = 0;

            _rippleFullscreen.material.SetFloat("_WaveDistanceFromCenter", -0.1f);
            _rippleFullscreen.material.DOFloat(1, "_WaveDistanceFromCenter", 1f).OnComplete(() => _rippleFullscreen.material.SetFloat("_WaveDistanceFromCenter", -0.1f));

            _shakeManager.addShakeWithPriority(6, 1, 0.2f, 2);
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            explosionParticle.GetComponent<ParticleSystem>().Play();
            for (int i = 0; i < disableOnDeath.Length; i++)
            {
                disableOnDeath[i].SetActive(false);
            }
        } else {
            GameObject _damageIndicatorClone = Instantiate(stats.damageIndicator);
            Destroy(_damageIndicatorClone, 5f);
            _damageIndicatorClone.transform.SetParent(enemyHUD.transform);
            _damageIndicatorClone.GetComponent<TextMeshProUGUI>().text = "-" + (healthBeforeAppliedDamage - currentHealth).ToString("F2");

            _damageIndicatorClone.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            _damageIndicatorClone.transform.eulerAngles = new Vector3(0, 0, Random.Range(-20f, 20f));
            _damageIndicatorClone.GetComponent<RectTransform>().DOAnchorPos(new Vector2(Random.Range(-1f, 1f), Random.Range(0.7f, 1.5f)), 0.5f).SetEase(Ease.OutBack);
            _damageIndicatorClone.GetComponent<TextMeshProUGUI>().DOFade(0, 0.5f).SetDelay(0.5f);

            _animator.Play("EnemyDamage");
            hitParticle.GetComponent<ParticleSystem>().Play();
        }

        radialHealth.fillAmount = Remap(currentHealth, 0f, stats.maxHealth, 0f,  1f);
    }
}
