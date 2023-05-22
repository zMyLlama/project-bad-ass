using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

public class EnemyController : MonoBehaviour
{
    [Header("Object Reference")]
    [InlineEditor(InlineEditorModes.FullEditor)]
    public EnemyStats_Settings stats;
    public UnityEvent primaryAttackEvent;
    [Header("Objects")]
    public GameObject[] disableOnDeath = new GameObject[ ]{};
    [ChildGameObjectsOnly(IncludeSelf = false)] public SpriteRenderer mySprite;
    [ChildGameObjectsOnly(IncludeSelf = false)] public Canvas enemyHUD;
    [ChildGameObjectsOnly(IncludeSelf = false)] public Image radialHealth;
    [Header("Particles")]
    public GameObject explosionParticle;
    public GameObject chargeParticle;
    public GameObject hitParticle;

    [HideInInspector] public Transform target;
    [HideInInspector] public SpriteRenderer rippleFullscreen; 
    [HideInInspector] public ShakeManager shakeManager;
    GameObject _worldCanvas;
    Animator _animator;
    Rigidbody2D _rb;
    Color _originalColor;
    float _currentHealth = 0.0f;
    bool _dead = false;

    float Remap(float value, float from1, float to1, float from2, float to2) {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    void Start()
    {
        _timeUntilNextAttack = Random.Range(stats.minPrimaryAttackCooldown, stats.maxPrimaryAttackCooldown);
        _rb = GetComponent<Rigidbody2D>();
        _currentHealth = stats.maxHealth;
        _originalColor = mySprite.color;
        _animator = GetComponent<Animator>();
        
        _worldCanvas = GameObject.FindGameObjectWithTag("World Canvas");
        target = GameObject.FindGameObjectWithTag("Player").transform;
        shakeManager = GameObject.FindGameObjectWithTag("Cinemachine").GetComponent<ShakeManager>();
        rippleFullscreen = GameObject.FindGameObjectWithTag("Cinemachine").transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>();
    }

    private IEnumerator ShootFireball() {
        shakeManager.addShakeWithPriority(1, 1, stats.timeBetweenChargeAndFireball, 1);
        chargeParticle.GetComponent<ParticleSystem>().Play();
        
        yield return new WaitForSeconds(stats.timeBetweenChargeAndFireball);

        chargeParticle.GetComponent<ParticleSystem>().Stop();

        for (int i = 0; i < 15; i++)
        {
            shakeManager.addShakeWithPriority(2, 1, 0.1f, 10);

            primaryAttackEvent.Invoke();
            GameObject fireball = Instantiate(stats.primaryProjectilePrefab, transform.position, Quaternion.identity);
            fireball.GetComponent<Fireball>().bulletSettings.target = target.transform.position;

            yield return new WaitForSeconds(0.1f);
        }

        cd = 0;
    }

    private float _knockbackTimer = 0;
    private float _knockbackForce;
    private Vector2 _knockbackDirection; 

    [HideInInspector] public float cd = 0;
    float _timeUntilNextAttack = 5;
    void Update()
    {
        if (_dead) return;
        if (cd == -1) _rb.velocity = new Vector2(0, 0);

        Vector2 direction = (target.position - transform.position).normalized;
        if (_knockbackTimer > 0)
        {
            _rb.AddForce(_knockbackDirection * (_knockbackForce * 80) * Time.deltaTime, ForceMode2D.Impulse);
            _knockbackTimer -= Time.deltaTime;
        }

        if (cd != -1 && _knockbackTimer <= 0)
            _rb.velocity = direction * stats.speed;

        if (3f > Vector3.Distance(transform.position, target.position) && cd != -1 && _knockbackTimer <= 0 && stats.fightingStyle == FightingStyles.Normal)
            _rb.velocity = direction * (stats.speed * 0.5f);

        if (stats.fleeRange > Vector3.Distance(transform.position, target.position) && cd != -1 && _knockbackTimer <= 0 && stats.fightingStyle == FightingStyles.Coward)
            _rb.velocity = direction / stats.fleeSpeed;

        if (cd != -1) cd += Time.deltaTime;
        if(cd >= _timeUntilNextAttack && cd != -1)
        {
            cd = -1;
            _timeUntilNextAttack = Random.Range(stats.minPrimaryAttackCooldown, stats.maxPrimaryAttackCooldown);
            primaryAttackEvent.Invoke();
        }
    }

    IEnumerator FlashWhite() {
        mySprite.color = Color.white;
        yield return new WaitForSeconds(0.2f);
        mySprite.color = _originalColor;
    }

    public void ApplyKnockback(Vector2 direction, float force, float duration) {
        _knockbackDirection = direction;
        _knockbackForce = force;
        _knockbackTimer = duration;
    }

    public void TakeDamage(float amount) {
        if (_dead) return;

        float healthBeforeAppliedDamage = _currentHealth;
        GameObject sfx = Instantiate(Resources.Load("SFX/EnemyHitAudio", typeof(GameObject)), transform.position, Quaternion.identity) as GameObject;
        sfx.GetComponent<AudioSource>().pitch = Random.Range(1f, 2f);
        sfx.transform.SetParent(GameObject.FindGameObjectWithTag("Cleaner").gameObject.transform);
        Destroy(sfx, 1f);

        _currentHealth -= amount;
        StartCoroutine("FlashWhite");
        if (_currentHealth <= 0) {
            _dead = true;
            Destroy(gameObject, 1f);
            gameObject.transform.SetParent(GameObject.FindGameObjectWithTag("Cleaner").transform);
            _currentHealth = 0;

            rippleFullscreen.material.SetFloat("_WaveDistanceFromCenter", -0.1f);
            rippleFullscreen.material.DOFloat(1, "_WaveDistanceFromCenter", 1f).OnComplete(() => rippleFullscreen.material.SetFloat("_WaveDistanceFromCenter", -0.1f));

            shakeManager.addShakeWithPriority(6, 1, 0.2f, 2);
            _rb.constraints = RigidbodyConstraints2D.FreezeAll;
            explosionParticle.GetComponent<ParticleSystem>().Play();
            for (int i = 0; i < disableOnDeath.Length; i++)
            {
                disableOnDeath[i].SetActive(false);
            }
        } else {
            GameObject damageIndicatorClone = Instantiate(stats.damageIndicator, _worldCanvas.transform, true);
            Destroy(damageIndicatorClone, 5f);
            damageIndicatorClone.GetComponent<TextMeshProUGUI>().text = "-" + (healthBeforeAppliedDamage - _currentHealth).ToString("F2");

            damageIndicatorClone.GetComponent<RectTransform>().position = gameObject.transform.position;
            damageIndicatorClone.transform.eulerAngles = new Vector3(0, 0, Random.Range(-20f, 20f));
            damageIndicatorClone.GetComponent<RectTransform>().DOMove(gameObject.transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(0.7f, 1.5f), 0), 0.5f).SetEase(Ease.OutBack);
            damageIndicatorClone.GetComponent<TextMeshProUGUI>().DOFade(0, 0.5f).SetDelay(0.5f);

            _animator.Play("EnemyDamage");
            hitParticle.GetComponent<ParticleSystem>().Play();
        }

        radialHealth.fillAmount = Remap(_currentHealth, 0f, stats.maxHealth, 0f,  1f);
    }
}
