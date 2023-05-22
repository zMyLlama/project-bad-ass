using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGunAttack : MonoBehaviour
{
    [Header("Settings")]
    public int bulletsAmount = 15;
    public float timeBetweenEachShot = 0.05f;

    float Remap(float value, float from1, float to1, float from2, float to2) {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public void LaunchAttack(EnemyController sender)
    {
        IEnumerator startAttack() {
            sender.shakeManager.addShakeWithPriority(1, 1, sender.stats.timeBetweenChargeAndFireball, 1);
            sender.chargeParticle.GetComponent<ParticleSystem>().Play();
            
            yield return new WaitForSeconds(sender.stats.timeBetweenChargeAndFireball);

            sender.chargeParticle.GetComponent<ParticleSystem>().Stop();

            for (int i = 0; i < bulletsAmount; i++)
            {
                sender.shakeManager.addShakeWithPriority(2, 1, 0.1f, 10);

                GameObject fireball = Instantiate(sender.stats.primaryProjectilePrefab, transform.position, Quaternion.identity);

                GameObject SFX = Instantiate(Resources.Load("SFX/EnemyShootAudio", typeof(GameObject)), transform.position, Quaternion.identity) as GameObject;
                SFX.transform.SetParent(GameObject.FindGameObjectWithTag("Cleaner").gameObject.transform);
                Destroy(SFX, 1f);


                Vector2 _toPosition = sender.target.position;
                Vector2 _fromPosition = transform.position;
                fireball.transform.right = _toPosition - _fromPosition;
                fireball.GetComponent<Fireball>().bulletSettings.target = sender.target.transform.position;

                yield return new WaitForSeconds(Remap(i, 0, bulletsAmount, timeBetweenEachShot, timeBetweenEachShot * 2));
            }

            sender.cd = 0;
        }

        StartCoroutine(startAttack());
    }
}
