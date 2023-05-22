using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class RadialAttack : MonoBehaviour
{
    [Title("Settings", "Adjust settings of attack pattern for given enemy")]
    [InfoBox("One means that the attack will only be done once. Increase to make it fire in circles multiple times.")] [Range(1, 10)] [SerializeField] int _attackRepetitions = 1;
    [Space(10f)] [Range(2, 20)] [SerializeField] int _amount = 2;
    [Range(0f, 10f)] [SerializeField] float _radius = 0;


    public void LaunchAttack(EnemyController sender)
    {
        IEnumerator startAttack() {
            sender.shakeManager.addShakeWithPriority(1, 1, sender.stats.timeBetweenChargeAndFireball / 2f, 1);
            sender.chargeParticle.GetComponent<ParticleSystem>().Play();

            yield return new WaitForSeconds(sender.stats.timeBetweenChargeAndFireball / 2f);

            sender.chargeParticle.GetComponent<ParticleSystem>().Stop();
            
            for (int j = 0; j < _attackRepetitions; j++)
            {    
                for (int i = 0; i < _amount; i++)
                {
                    var radians = 2 * Mathf.PI / _amount * i;
                    var vertical = Mathf.Sin(radians);
                    var horizontal = Mathf.Cos(radians); 

                    GameObject bullet = Instantiate(sender.stats.primaryProjectilePrefab, transform.position, Quaternion.identity);
                    bullet.transform.position += new Vector3(horizontal, vertical, 0);
                    bullet.transform.right = -(transform.position - bullet.transform.position);
                    bullet.GetComponent<Fireball>().bulletSettings.target = transform.position + new Vector3(horizontal, vertical, 0) * (_radius * 2);
                    sender.shakeManager.addShakeWithPriority(3, 1, 0.1f, 12);

                    GameObject SFX = Instantiate(Resources.Load("SFX/EnemyShootAudio", typeof(GameObject)), transform.position, Quaternion.identity) as GameObject;
                    SFX.transform.SetParent(GameObject.FindGameObjectWithTag("Cleaner").gameObject.transform);
                    Destroy(SFX, 1f);

                    yield return new WaitForSeconds((sender.stats.timeBetweenChargeAndFireball / 2f) / _amount);
                }
            }

            yield return new WaitForSeconds(0.5f);

            sender.cd = 0;
        }

        StartCoroutine(startAttack());
    }

    private void OnDrawGizmos() {
        //if (UnityEditor.Selection.activeGameObject != this.gameObject) return;

        for (int i = 0; i < _amount; i++) {
            var radians = 2 * Mathf.PI / _amount * i;
            var vertical = Mathf.Sin(radians);
            var horizontal = Mathf.Cos(radians); 

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + new Vector3(horizontal, vertical, 0) * _radius, 0.25f);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position + new Vector3(horizontal, vertical, 0) * _radius, transform.position + new Vector3(horizontal, vertical, 0) * (_radius * 2));
        }
    }
}
