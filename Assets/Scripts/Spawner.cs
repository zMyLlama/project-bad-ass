using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

public class Spawner : MonoBehaviour
{
    [Header("Settings")]
    [InlineEditor(InlineEditorModes.FullEditor)]
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

        GameObject _spawnCloud = Instantiate(Resources.Load("EnemySpawnFXV2", typeof(GameObject)), gameObject.transform.position, Quaternion.identity) as GameObject;
        _spawnCloud.transform.SetParent(GameObject.FindGameObjectWithTag("Cleaner").transform, true);
        Destroy(_spawnCloud, 1.5f);

        GameObject SFX = Instantiate(Resources.Load("SFX/EnemySpawnAudio", typeof(GameObject)), transform.position, Quaternion.identity) as GameObject;
        SFX.GetComponent<AudioSource>().pitch = Random.Range(0.7f, 1.3f);
        SFX.transform.SetParent(GameObject.FindGameObjectWithTag("Cleaner").gameObject.transform);
        Destroy(SFX, 1f);

        GameObject.FindGameObjectWithTag("Cinemachine").GetComponent<ShakeManager>().addShakeWithPriority(0.5f, 1, 0.2f, 1);
    }

    private void Awake() {
        this.gameObject.SetActive(false);
    }

    private void Start() {
        StartCoroutine("instantiateEnemy");
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.25f);
    }
}
