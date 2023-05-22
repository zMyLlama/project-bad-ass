using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

public class Fireball : MonoBehaviour
{ 
  public Bullet_Settings bulletExtendsTo;
  [HideInInspector] public Bullet_Settings bulletSettings;

  private Vector3 direction;

  private void Awake() {
    bulletSettings = bulletExtendsTo;
  }

  private void Start() {
    transform.localScale = bulletSettings.scale / 2;
    transform.DOScale(bulletSettings.scale, 0.5f).SetEase(Ease.OutBack).OnComplete(() => 
      transform.DOScale(bulletSettings.scale * 1.5f, 0.15f).SetLoops(-1, LoopType.Yoyo)
    );

    direction = (bulletSettings.target - transform.position).normalized;
    Destroy(gameObject, 5f);
  }

  private void Update()
  {
    transform.position += direction * bulletSettings.speed * Time.deltaTime;
  }

  private void OnTriggerEnter2D(Collider2D other) {
    if (other.tag != "Player") {
      Destroy(gameObject);
      return;
    }
    if (other.gameObject.GetComponent<Movement>()._isDashing) return;
    
    other.gameObject.GetComponent<Combat>().DamagePlayer(bulletSettings.baseDamage);
    Destroy(gameObject);
  }
}