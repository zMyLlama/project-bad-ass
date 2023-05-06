using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class Combat : MonoBehaviour
{
    [Header("Health")]
    public int hearts = 5;
    public int amountOfHeartStates = 4;
    public Sprite[ ] heartStates = new Sprite[  ]{};
    [HideInInspector] public float health = 0;

    [Header("Attack stats")]
    [Range(0.2f, 5f)] public float slowestAttackSpeed = 2f;
    [Range(0.2f, 5f)] public float fastestAttackSpeed = 0.2f;
    [Range(0.2f, 5f)] public float currentSwordAttackSpeed = 0.2f;
    public float maxiumumDamage = 12.5f;
    public float minimumDamage = 5f;
    public float attackCooldown = 0.15f;

    [Header("Objects")]
    public GameObject heartsHolder;
    public GameObject heart;
    public Sprite fullHeartSprite;
    public ShakeManager shakeManager;
    public Animator weaponAnimator;
    public Volume globalVolume;

    private Vignette vignetteEffect;
    private List<GameObject> heartObjects = new List<GameObject>();

    float Remap(float value, float from1, float to1, float from2, float to2) {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    private void Awake() {
        for (int i = 0; i < hearts; i++)
        {
            GameObject _heartClone = Instantiate(heart);
            _heartClone.name = "Heart_" + i;
            _heartClone.transform.SetParent(heartsHolder.transform);
            _heartClone.transform.localScale = new Vector3(1, 1, 1);

            heartObjects.Add(_heartClone);
        }
    }

    private void Start() {
        health = hearts * amountOfHeartStates;

        globalVolume.profile.TryGet(out vignetteEffect);
    }

    float _vignetteEffectForce = 0f;
    int _damagedPulsingState = 0; /* 0 is off */
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            damagePlayer(1);
        }
        if (Input.GetMouseButtonDown(1)) {
            damagePlayer(-1);
        }

        if (health < (2 * amountOfHeartStates) && health > amountOfHeartStates && _damagedPulsingState != 1 && _vignetteEffectForce < 0.01f) {
            _damagedPulsingState = 1;

            DOTween.Kill("DamagedPulsingLeaving");
            DOTween.Kill("DamagedPulsing");
            DOTween.To(() => _vignetteEffectForce, x => _vignetteEffectForce = x, 0.5f, 1f).SetLoops(-1, LoopType.Yoyo).OnUpdate(() => vignetteEffect.intensity.value = _vignetteEffectForce).SetId("DamagedPulsing");
        } else if (health <= amountOfHeartStates && _damagedPulsingState != 2 && _vignetteEffectForce < 0.01f) {
            _damagedPulsingState = 2;

            DOTween.Kill("DamagedPulsingLeaving");
            DOTween.Kill("DamagedPulsing");
            DOTween.To(() => _vignetteEffectForce, x => _vignetteEffectForce = x, 0.5f, 0.5f).SetLoops(-1, LoopType.Yoyo).OnUpdate(() => vignetteEffect.intensity.value = _vignetteEffectForce).SetId("DamagedPulsing");
        } else if (health >= (2 * amountOfHeartStates) && _damagedPulsingState != 0) {
            _damagedPulsingState = 0;

            DOTween.Kill("DamagedPulsing");
            DOTween.To(() => _vignetteEffectForce, x => _vignetteEffectForce = x, 0f, 1f).OnUpdate(() => vignetteEffect.intensity.value = _vignetteEffectForce).SetId("DamagedPulsingLeaving");
        }
    }

    IEnumerator damagePostEffect() {
        vignetteEffect.intensity.value = 0.5f;
        yield return new WaitForSeconds(0.2f);
        vignetteEffect.intensity.value = 0f;
    }

    public void damagePlayer(int amount) {
        health -= amount;
        //StartCoroutine(damagePostEffect());

        int totalIndex = 0;
        for (int i = 0; i < Mathf.Floor(health / amountOfHeartStates); i++)
        {
            GameObject _heartClone = heartsHolder.transform.GetChild(totalIndex).gameObject;
            _heartClone.GetComponent<Image>().sprite = fullHeartSprite;
            totalIndex++;
        }
        if (health % amountOfHeartStates != 0.0f) {
            GameObject _heartClone = heartsHolder.transform.GetChild(totalIndex).gameObject;
            _heartClone.GetComponent<Image>().sprite = heartStates[Mathf.RoundToInt(health % amountOfHeartStates)];
            totalIndex++;
        }
        for (int i = 0; i < (hearts - Mathf.Ceil(health / amountOfHeartStates)); i++)
        {
            if (!heartsHolder.transform.GetChild(totalIndex)) continue;
            GameObject _heartClone = heartsHolder.transform.GetChild(totalIndex).gameObject;
            _heartClone.GetComponent<Image>().sprite = heartStates[0];
            totalIndex++;
        }
    }

    public void swordCollisionEvent(Collider2D other) {
        if (other.tag != "Enemy") return;
        Vector2 _direction = (other.gameObject.transform.position - transform.position).normalized;
        other.gameObject.GetComponent<EnemyController>().applyKnockback(_direction, 2f, 0.1f);

        other.gameObject.GetComponent<EnemyController>().takeDamage(Remap(currentSwordAttackSpeed, fastestAttackSpeed, slowestAttackSpeed, maxiumumDamage, minimumDamage));
        shakeManager.addShakeWithPriority(2, 1, 0.1f, 1);
    }
}
