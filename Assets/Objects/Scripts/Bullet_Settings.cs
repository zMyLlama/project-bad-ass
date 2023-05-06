using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Bullet", menuName = "Bullet")]
public class Bullet_Settings : ScriptableObject
{
    public float speed = 10f;
    public float baseDamage = 2f;
    public Vector3 scale = new Vector3(1f, 1f, 1f);
    [HideInInspector] public Vector3 target;
}
