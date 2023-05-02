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
      //Det her er til at se hvor spilleren st�r i det �jeblik den bliver skudt.
    }

    private void Update()
    {
    transform.position = Vector3.MoveTowards(transform.position, target, Speed * Time.deltaTime);
     //Det her er til at bev�ge sig til spilleren x,y og z koordinator n�r ildkulen bliver "skudt"/spawner

        if(target == transform.position)
        {
            Destroy(this.gameObject);
            //Det her er til at fjerne/slette ildkulen n�r den n�r til spillerens position, s� spillet ikke bliver fyldt op af ildkugler.
        }
    }
}