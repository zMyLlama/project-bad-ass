using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using DG.Tweening;
using Sirenix.OdinInspector;

public class Combat : MonoBehaviour
{
    [TitleGroup("Health", "Manage the health settings of the player", TitleAlignments.Centered, BoldTitle = true)]
    [TabGroup("Health/Objects", "Heart Objects")] [PropertyOrder(1)]
    public Sprite[ ] heartStates = new Sprite[  ]{};
    [TabGroup("Health/Objects", "Health Settings")] [PropertyOrder(0)]
    public int hearts = 5;
    [TabGroup("Health/Objects", "Health Settings")] [PropertyOrder(0)]
    public int amountOfHeartStates = 4;
    [PropertySpace(10)] [InfoBox("Following values are only read for debug purposes.")]
    [TabGroup("Health/Objects", "Health Settings")] [PropertyOrder(0)] [ReadOnly]
    public float health = 0;
    [TabGroup("Health/Objects", "Health Settings")] [PropertyOrder(0)] [ShowIf("@this._dead == true")] [ReadOnly]
    public bool _dead = false;

    [TitleGroup("Attack", "Manage the attack settings of the player", TitleAlignments.Centered, BoldTitle = true)]
    [HorizontalGroup("Attack/AttackSpeedSplit", MarginRight = 50)] [Range(0.2f, 4f)] public float slowestAttackSpeed = 2f;
    [HorizontalGroup("Attack/AttackSpeedSplit")] [Range(0.2f, 4f)] public float fastestAttackSpeed = 0.2f;
    [MinMaxSlider(0f, 50f, true)] public Vector2 minMaxDamage = new Vector2(5f, 12.5f);
    [PropertySpace(10)] [InfoBox("Following values are only read for debug purposes.")]
    [ReadOnly] public float currentSwordAttackSpeed = 0.2f;

    [TitleGroup("Objects", "All external references to objects", TitleAlignments.Centered, BoldTitle = true)]
    public GameObject deathCanvas;
    public Image fade;
    public RawImage skull;
    public GameObject heartsHolder;
    public GameObject heart;
    public ShakeManager shakeManager;
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

    IEnumerator deathAnimation() {
        shakeManager.killAllScreenshake();
        shakeManager.preventFurtherScreenshake = true;

        GameObject _canvas = GameObject.FindGameObjectWithTag("Canvas").gameObject;
        _canvas.SetActive(false);
        fade.DOFade(1, 0.25f);

        yield return new WaitForSeconds(0.4f);

        skull.gameObject.GetComponent<RectTransform>().position = gameObject.transform.position;
        skull.gameObject.SetActive(true);

        skull.DOFade(1, 0.3f);
        skull.gameObject.GetComponent<RectTransform>().DOScale(new Vector3(0.01f, 0.01f, 1f), 0.35f).OnComplete(() => shakeManager.addShakeWithPriority(10, 3, 0.3f, 1000, true));

        yield return new WaitForSeconds(1.4f);

        shakeManager.addShakeWithPriority(5, 2, 0.2f, 1000, true);
        skull.DOFade(0, 0.1f);
        skull.transform.GetChild(0).GetComponent<ParticleSystem>().Play();

        yield return new WaitForSeconds(1.5f);

        deathCanvas.SetActive(true);
        _canvas.SetActive(true);
    }

    IEnumerator damagePostEffect() {
        vignetteEffect.intensity.value = 0.5f;
        yield return new WaitForSeconds(0.2f);
        vignetteEffect.intensity.value = 0f;
    }

    public void damagePlayer(int amount) {
        if (_dead) return;

        health -= amount;
        if (health <= 0 && !_dead) {
            _dead = true;
            StartCoroutine(deathAnimation());
        };

        int totalIndex = 0;
        for (int i = 0; i < Mathf.Floor(health / amountOfHeartStates); i++)
        {
            GameObject _heartClone = heartsHolder.transform.GetChild(totalIndex).gameObject;
            _heartClone.GetComponent<Image>().sprite = heartStates[4];
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

        other.gameObject.GetComponent<EnemyController>().takeDamage(Remap(currentSwordAttackSpeed, fastestAttackSpeed, slowestAttackSpeed, minMaxDamage.y, minMaxDamage.x));
        shakeManager.addShakeWithPriority(2, 1, 0.1f, 1);
    }
}
