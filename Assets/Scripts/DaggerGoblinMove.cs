using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaggerGoblinMove : MonoBehaviour
{
    public int Speed;
    public int hp;

    Rigidbody2D rigidbody;

    public Transform Player;
    public float detectionrange;
    Transform currentTarget;


    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();

        Speed = 8;
        hp = 3;

        currentTarget = Player;

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = currentTarget.position - transform.position;
        direction.Normalize();
        rigidbody.velocity = direction * Speed;

    }


}
