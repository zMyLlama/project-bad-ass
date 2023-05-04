using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class EnemyController : MonoBehaviour
{
    enum EnemyTypes { Wizard, Snake }
    enum FightingStyles { Normal, Coward, Aggressive, PassiveAggressive }

    [Header("Settings")]
    [Tooltip("Changes the behavior of how the enemy works. Other settings will still have to be manually set.")] [SerializeField] EnemyTypes enemyType;
    public float maxHealth = 50.0f;
    public int Speed = 4;
    public float runSpeed = -0.3f;
    public int HP = 5;
    public float runRange = 6;
    [SerializeField] float timeBetweenChargeAndFireball = 1.5f;
    [Header("Hostility")]
    [SerializeField] FightingStyles fightingStyle;
    [Header("Objects")]
    public GameObject[] disableOnDeath = new GameObject[ ]{};
    public SpriteRenderer mySprite;
    public GameObject fireballPrefab;
    public GameObject damageIndicator;
    public GameObject enemyHUD;
    public Image radialHealth;
    [Header("Particles")]
    public GameObject explosionParticle;
    public GameObject chargeParticle;
    public GameObject hitParticle;

    Transform _target;
    SpriteRenderer _rippleFullscreen; 
    ShakeManager _shakeManager;
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
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        originalColor = mySprite.color;
        _animator = GetComponent<Animator>();

        _target = GameObject.FindGameObjectWithTag("Player").transform;
        _shakeManager = GameObject.FindGameObjectWithTag("Cinemachine").GetComponent<ShakeManager>();
        _rippleFullscreen = GameObject.FindGameObjectWithTag("Cinemachine").transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>();
    }

    IEnumerator shootFireball() {
        _shakeManager.addShakeWithPriority(1, 1, timeBetweenChargeAndFireball, 1);
        chargeParticle.GetComponent<ParticleSystem>().Play();
        
        yield return new WaitForSeconds(timeBetweenChargeAndFireball);

        _shakeManager.addShakeWithPriority(4, 2, 0.2f, 10);
        chargeParticle.GetComponent<ParticleSystem>().Stop();

        GameObject fireball = Instantiate(fireballPrefab, transform.position, Quaternion.identity);
        fireball.GetComponent<Fireball>().Player = _target;
        CD = 0;
    }

    float CD = 0;
    int timeUntilNextAttack = 5;
    void Update()
    {
        if (dead) return;
        if (CD == -1) rb.velocity = new Vector2(0, 0);

        Vector3 direction = _target.position - transform.position;
        direction.Normalize();
        if (CD != -1) rb.velocity = direction * Speed;

        if (runRange > Vector3.Distance(transform.position, _target.position) && CD != -1)
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
            GameObject _damageIndicatorClone = Instantiate(damageIndicator);
            Destroy(_damageIndicatorClone, 5f);
            _damageIndicatorClone.transform.SetParent(enemyHUD.transform);
            _damageIndicatorClone.GetComponent<TextMeshProUGUI>().text = (healthBeforeAppliedDamage - currentHealth).ToString("F2");

            _damageIndicatorClone.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            _damageIndicatorClone.transform.eulerAngles = new Vector3(0, 0, Random.Range(-20f, 20f));
            _damageIndicatorClone.GetComponent<RectTransform>().DOAnchorPos(new Vector2(Random.Range(-1f, 1f), Random.Range(0.7f, 1.5f)), 0.5f).SetEase(Ease.OutBack);
            _damageIndicatorClone.GetComponent<TextMeshProUGUI>().DOFade(0, 0.5f).SetDelay(0.5f);

            _animator.Play("EnemyDamage");
            hitParticle.GetComponent<ParticleSystem>().Play();
        }

        radialHealth.fillAmount = Remap(currentHealth, 0f, maxHealth, 0f,  1f);
    }
}
