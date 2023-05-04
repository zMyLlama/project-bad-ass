using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cinemachine;
using DG.Tweening;

public class Startup : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] bool developmentMode = false;
    [SerializeField] float orthographicGoToSize = 6f;
    [SerializeField] string welcomeMessage = "THE DUNGEON";
    [Header("Objects")]
    [SerializeField] Image screenCover;
    [SerializeField] TMP_Text welcomeText;

    CinemachineVirtualCamera cinemachineVirtualCamera;
    char[] characters = {'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '!', '@', '#', '$', '%', '^', '&', '_', '-'};

    IEnumerator textGlitchEffect() {
        string _hackedString = "";
        for (int i = 0; i < welcomeMessage.Length; i++)
        {
            if (welcomeMessage[i] == ' ') {
                _hackedString += ' ';
                continue;
            }

            char randomCharacter = characters[Random.Range(0, characters.Length)];
            _hackedString += randomCharacter;
        }
        welcomeText.text = _hackedString;
        welcomeText.gameObject.SetActive(true);

        for (int j = 0; j < welcomeMessage.Length; j++)
        {
            for (int k = 0; k < Random.Range(3, 8); k++)
            {    
                string _finalString = "";
                for (int i = 0; i < welcomeMessage.Length; i++)
                {
                    if (welcomeMessage[i] == ' ') {
                        _finalString += ' ';
                        continue;
                    }
                    if (i <= j && j != 0) {
                        _finalString += welcomeMessage[i];
                        continue;
                    }

                    char randomCharacter = characters[Random.Range(0, characters.Length)];
                    _finalString += randomCharacter;
                }

                welcomeText.text = _finalString;
                yield return new WaitForSeconds(0.015f);
            }
        }

        yield return new WaitForSeconds(1.5f);

        welcomeText.DOFade(0, 1f).SetEase(Ease.OutCirc);
        welcomeText.GetComponent<RectTransform>().DOAnchorPosY(100f, 2f).SetEase(Ease.OutCirc);
    }

    private void Awake() {
        if (developmentMode) return;
        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();

        screenCover.color = new Color32(0, 0, 0, 255);
        float _currentOrthographicSize = 1f;
        DOTween.To(() => _currentOrthographicSize, x => _currentOrthographicSize = x, orthographicGoToSize, 5f ).SetDelay(4f).SetEase(Ease.InOutQuart).OnUpdate(() => cinemachineVirtualCamera.m_Lens.OrthographicSize = _currentOrthographicSize);
        screenCover.DOFade(0, 6f).SetDelay(4.5f).SetEase(Ease.OutCubic);

        StartCoroutine("textGlitchEffect");
    }
}
