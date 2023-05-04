using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemiesLeft : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] string label = "ENEMIES LEFT";
    [Header("Objects")]
    [SerializeField] GameObject enemiesHolder;

    TMP_Text _text;

    private void Awake() {
        _text = GetComponent<TextMeshProUGUI>();
    }

    private void FixedUpdate() {
        _text.text = label + ": " + enemiesHolder.transform.childCount;
    }
}
