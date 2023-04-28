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
      //Det her er til at se hvor spilleren står i det øjeblik den bliver skudt.
    }

    private void Update()
    {
    transform.position = Vector3.MoveTowards(transform.position, target, Speed * Time.deltaTime);
     //Det her er til at bevæge sig til spilleren x,y og z koordinator når ildkulen bliver "skudt"/spawner

        if(target == transform.position)
        {
            Destroy(this.gameObject);
            //Det her er til at fjerne/slette ildkulen når den når til spillerens position, så spillet ikke bliver fyldt op af ildkugler.
        }
    }
}