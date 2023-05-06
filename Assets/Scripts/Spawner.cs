using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Spawner : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] GameObject enemyToSpawn;

    IEnumerator instantiateEnemy() {
        GameObject _enemyClone = Instantiate(enemyToSpawn, gameObject.transform.position, Quaternion.identity);
        _enemyClone.GetComponent<Animator>().enabled = false;
        _enemyClone.SetActive(false);
        _enemyClone.transform.SetParent(GameObject.FindGameObjectWithTag("Enemies").transform, true);

        yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));

        _enemyClone.SetActive(true);
        _enemyClone.transform.localScale = new Vector2(0, 0);
        _enemyClone.transform.DOScale(new Vector2(1f, 1f), 0.6f).SetEase(Ease.OutBack).OnComplete(() => _enemyClone.GetComponent<Animator>().enabled = true);

        GameObject _spawnCloud = Instantiate(Resources.Load("EnemySpawnFX", typeof(GameObject)), gameObject.transform.position, Quaternion.identity) as GameObject;
        _spawnCloud.transform.SetParent(GameObject.FindGameObjectWithTag("Cleaner").transform, true);
        Destroy(_spawnCloud, 1.5f);

        GameObject.FindGameObjectWithTag("Cinemachine").GetComponent<ShakeManager>().addShakeWithPriority(0.5f, 1, 0.2f, 1);
    }

    private void Start() {
        StartCoroutine("instantiateEnemy");
    }
}
