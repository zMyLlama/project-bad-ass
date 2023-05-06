using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Stats", menuName = "Enemy Stats")]
public class EnemyStats_Settings : ScriptableObject
{
    [HideInInspector] public enum FightingStyles { Normal, Coward, Aggressive, PassiveAggressive }

    [Header("Settings")]
    public string enemyName = "Ben";
    [Space(10)]
    public float maxHealth = 100f;
    public int speed = 100;
    public FightingStyles fightingStyle = FightingStyles.Normal;
    [Space(10)]
    public float fleeSpeed = -0.3f;
    public float fleeRange = 6f;
    [Space(10)]
    public float timeBetweenChargeAndFireball = 1.5f;
    [Space(10)]

    [Header("Objects")]
    public GameObject damageIndicator;
    public GameObject primaryProjectilePrefab;

}
