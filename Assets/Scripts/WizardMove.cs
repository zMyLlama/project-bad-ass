using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardMove : MonoBehaviour
{
    //Script til mage fjende.

    public int Speed;
    private float RunSpeed;
    public int hp;

    Rigidbody2D rigidbody;

    public Transform Player;
    public float detectionrange;
    public float runrange;
    Transform currentTarget;

    float CD = 0;

    public GameObject fireballPrefab;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();

        currentTarget = Player;
        runrange = 6;

        hp = 5;
        Speed = 4;
        RunSpeed = -0.3f;

        //Her indlæser jeg rigidbodyen samt bestæmmer hastighed, liv og gør så den kan løbe fra spilleren.
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = currentTarget.position - transform.position;
        direction.Normalize();
        rigidbody.velocity = direction * Speed;
        // Det her er til at bevæge sig imod spilleren

        if (runrange > Vector3.Distance(transform.position, Player.position))
        {
            rigidbody.velocity = direction / RunSpeed;
            // Det her er så at magen flygter/løber fra spilleren hvis den kommer for tæt på.
        }

        CD += Time.deltaTime;
        if(CD >= 5)
        {
            GameObject fireball = Instantiate(fireballPrefab, transform.position, Quaternion.identity);
            fireball.GetComponent<Fireball>().Player = Player;
            CD = 0;
            // Det her er til at magen skyder  en ildkule imod spilleren med et interval i sekunder.
        }
    }
}
