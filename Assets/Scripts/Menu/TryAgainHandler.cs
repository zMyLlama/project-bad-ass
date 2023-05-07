using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using DG.Tweening;

public class TryAgainHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    AudioSource hoverSFX;
    AudioSource clickSFX;
    AudioSource musicSFX;

    private void Start() {
        hoverSFX = transform.parent.GetChild(transform.parent.childCount - 1).GetChild(2).GetComponent<AudioSource>();
        clickSFX = transform.parent.GetChild(transform.parent.childCount - 1).GetChild(1).GetComponent<AudioSource>();
        musicSFX = transform.parent.GetChild(transform.parent.childCount - 1).GetChild(3).GetComponent<AudioSource>();
    }

    IEnumerator retryGame() {
        yield return 0;
    }

    public void OnPointerClick(PointerEventData pointerEventData) {
        StartCoroutine(retryGame());
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        hoverSFX.Play();
        GetComponent<RectTransform>().DOScale(new Vector3(2.2f, 2.2f, 2.2f), 0.3f);
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        GetComponent<RectTransform>().DOScale(new Vector3(2f, 2f, 2f), 0.3f);
    }
}
