using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{ 
  public float Speed = 10f;

    public Transform Player;

    private Vector3 target;

    private void Start()
    {
    target = Player.transform.position;

    }

    private void Update()
    {
    transform.position = Vector3.MoveTowards(transform.position, target, Speed * Time.deltaTime);

        if(target == transform.position)
        {
            Destroy(this.gameObject);
        }
    }
}