using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public enum FightingStyles { Normal, Coward, Aggressive, PassiveAggressive }
public enum MovementStyles { Walking, Running, Jumping }

[CreateAssetMenu(fileName = "New Enemy Stats", menuName = "Enemy Stats")]
public class EnemyStats_Settings : ScriptableObject
{

    [Header("Settings")]
    public float maxHealth = 100f;
    [ProgressBar(0, 20)] public int speed = 100;
    [EnumToggleButtons] public MovementStyles movementStyle = MovementStyles.Walking;
    [EnumToggleButtons] public FightingStyles fightingStyle = FightingStyles.Normal;
    [Space(10)]
    [ShowIf("fightingStyle", FightingStyles.Coward)] public float fleeSpeed = -0.3f;
    [ShowIf("fightingStyle", FightingStyles.Coward)] public float fleeRange = 6f;
    [Space(10)]
    public float minPrimaryAttackCooldown = 1f;
    public float maxPrimaryAttackCooldown = 3f;
    [Space(10)]
    public float timeBetweenChargeAndFireball = 1.5f;
    [Space(10)]

    [Header("Objects")]
    public GameObject damageIndicator;
    public GameObject primaryProjectilePrefab;

}
