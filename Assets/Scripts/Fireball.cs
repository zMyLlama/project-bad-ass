using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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
    transform.DOScale(bulletSettings.scale, 0.5f).SetEase(Ease.OutBack);

    direction = (bulletSettings.target - transform.position).normalized;
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

    //other.gameObject

    //Destroy(gameObject);
  }
}