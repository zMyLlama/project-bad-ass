using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat : MonoBehaviour
{
    public Animator weaponAnimator;
    [Header("Attack stats")]
    [Range(0.2f, 5f)] public float slowestAttackSpeed = 2f;
    [Range(0.2f, 5f)] public float fastestAttackSpeed = 0.2f;
    [Range(0.2f, 5f)] public float currentSwordAttackSpeed = 0.2f;

    void Update()
    {
        
    }
}
