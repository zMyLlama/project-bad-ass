using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;

public class GoToMainMenuHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    Color32 originalColor;
    AudioSource hoverSFX;
    AudioSource clickSFX;
    AudioSource musicSFX;
    private void Start() {
        originalColor = GetComponent<Image>().color;
        originalColor.a = 255;
        hoverSFX = transform.parent.GetChild(transform.parent.childCount - 1).GetChild(2).GetComponent<AudioSource>();
        clickSFX = transform.parent.GetChild(transform.parent.childCount - 1).GetChild(1).GetComponent<AudioSource>();
        musicSFX = transform.parent.GetChild(transform.parent.childCount - 1).GetChild(3).GetComponent<AudioSource>();
    }

    IEnumerator goToMainMenu() {
        clickSFX.Play();
        transform.parent.GetChild(transform.parent.childCount - 2).GetComponent<Image>().DOFade(1, 1f);
        musicSFX.DOFade(0, 1.4f);

        yield return new WaitForSeconds(1.5f);

        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    public void OnPointerClick(PointerEventData pointerEventData) {
        StartCoroutine(goToMainMenu());
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        hoverSFX.Play();
        GetComponent<RectTransform>().DOScale(new Vector3(2.2f, 2.2f, 2.2f), 0.2f);
        GetComponent<Image>().DOColor(new Color32(255, 255, 255, 255), 0.2f);
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().DOColor(new Color32(255, 255, 255, 255), 0.2f);
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        GetComponent<RectTransform>().DOScale(new Vector3(2f, 2f, 2f), 0.3f);
        GetComponent<Image>().DOColor(originalColor, 0.3f);
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().DOColor(originalColor, 0.3f);
    }
}
