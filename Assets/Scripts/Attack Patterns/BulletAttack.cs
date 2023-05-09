using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BulletAttack : MonoBehaviour
{
    public void LaunchAttack(EnemyController sender)
    {
        IEnumerator startAttack() {
            sender._shakeManager.addShakeWithPriority(1, 1, sender.stats.timeBetweenChargeAndFireball, 1);
            sender.chargeParticle.GetComponent<ParticleSystem>().Play();
            
            yield return new WaitForSeconds(sender.stats.timeBetweenChargeAndFireball);

            sender.chargeParticle.GetComponent<ParticleSystem>().Stop();

            sender._shakeManager.addShakeWithPriority(4, 2, 0.1f, 11);

            GameObject fireball = Instantiate(sender.stats.primaryProjectilePrefab, transform.position, Quaternion.identity);
            fireball.transform.DORotate(new Vector3(0, 0, 360f), 0.6f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);
            fireball.GetComponent<Fireball>().bulletSettings.target = sender._target.transform.position;

            GameObject SFX = Instantiate(Resources.Load("SFX/EnemyShootAudio", typeof(GameObject)), transform.position, Quaternion.identity) as GameObject;
            SFX.transform.SetParent(GameObject.FindGameObjectWithTag("Cleaner").gameObject.transform);
            Destroy(SFX, 1f);

            sender.CD = 0;
        }

        StartCoroutine(startAttack());
    }
}
