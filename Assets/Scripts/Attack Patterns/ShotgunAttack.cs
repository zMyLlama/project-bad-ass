using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunAttack : MonoBehaviour
{
    [Header("Settings")]
    public float minSpreadX = -1;
    public float maxSpreadX = 1;
    public float minSpreadY = -1;
    public float maxSpreadY = 1;
    [Space(10)]
    public int bulletsAmount = 5;


    public void LaunchAttack(EnemyController sender)
    {
        IEnumerator startAttack() {
            sender._shakeManager.addShakeWithPriority(1, 1, sender.stats.timeBetweenChargeAndFireball, 1);
            sender.chargeParticle.GetComponent<ParticleSystem>().Play();
            
            yield return new WaitForSeconds(sender.stats.timeBetweenChargeAndFireball);

            sender.chargeParticle.GetComponent<ParticleSystem>().Stop();

            sender._shakeManager.addShakeWithPriority(6, 2, 0.1f, 12);

            GameObject SFX = Instantiate(Resources.Load("SFX/EnemyShootAudio", typeof(GameObject)), transform.position, Quaternion.identity) as GameObject;
            SFX.transform.SetParent(GameObject.FindGameObjectWithTag("Cleaner").gameObject.transform);
            Destroy(SFX, 1f);

            for (int i = 0; i < bulletsAmount; i++)
            {

                GameObject fireball = Instantiate(sender.stats.primaryProjectilePrefab, transform.position, Quaternion.identity);
                fireball.transform.position += new Vector3(Random.Range(minSpreadX, maxSpreadX), Random.Range(minSpreadY, maxSpreadY), 0);
                fireball.transform.right = -(transform.position - fireball.transform.position);
                fireball.GetComponent<Fireball>().bulletSettings.target = sender._target.transform.position;

                yield return new WaitForSeconds(0.04f);
            }

            sender.CD = 0;
        }

        StartCoroutine(startAttack());
    }
}
