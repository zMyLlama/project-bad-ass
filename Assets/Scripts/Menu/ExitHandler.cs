using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;

public class ExitHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public AudioSource hoverSFX;

    Color32 originalColor;
    private void Start() {
        originalColor = GetComponent<Image>().color;
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        hoverSFX.Play();
        GetComponent<RectTransform>().DOScale(new Vector3(2.2f, 2.2f, 2.2f), 0.2f);
        GetComponent<Image>().DOColor(new Color32(255, 255, 255, 255), 0.2f);
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().DOColor(new Color32(255, 255, 255, 255), 0.2f);
    }

    //Detect when Cursor leaves the GameObject
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        GetComponent<RectTransform>().DOScale(new Vector3(2f, 2f, 2f), 0.3f);
        GetComponent<Image>().DOColor(originalColor, 0.3f);
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().DOColor(originalColor, 0.3f);
    }
}
