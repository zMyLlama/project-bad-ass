using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class DeathEffect : MonoBehaviour
{
    void Start()
    {
        transform.GetChild(transform.childCount - 1).GetChild(3).GetComponent<AudioSource>().Play();
        transform.GetChild(transform.childCount - 1).GetChild(3).GetComponent<AudioSource>().DOFade(0.7f, 3f).SetDelay(2f);

        transform.GetChild(0).gameObject.GetComponent<Image>().color = new Color32(255, 255, 255, 0);
        transform.GetChild(0).gameObject.GetComponent<RectTransform>().localScale = new Vector3(1.1f, 1.1f, 1.1f);
        transform.GetChild(1).gameObject.GetComponent<RectTransform>().position -= new Vector3(0, 50f, 0);
        transform.GetChild(2).gameObject.GetComponent<RectTransform>().position -= new Vector3(0, 50f, 0);

        transform.GetChild(0).gameObject.GetComponent<Image>().DOFade(1, 0.5f);
        transform.GetChild(0).gameObject.GetComponent<RectTransform>().DOScale(new Vector3(1, 1, 1), 0.5f);

        transform.GetChild(1).gameObject.GetComponent<RectTransform>().DOMoveY(transform.GetChild(1).gameObject.GetComponent<RectTransform>().position.y + 50, 0.7f).SetDelay(0.4f);
        transform.GetChild(1).gameObject.GetComponent<Image>().DOFade(1, 0.5f).SetDelay(0.4f);
        transform.GetChild(1).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().DOFade(1, 0.5f).SetDelay(0.4f);

        transform.GetChild(2).gameObject.GetComponent<RectTransform>().DOMoveY(transform.GetChild(2).gameObject.GetComponent<RectTransform>().position.y + 50, 0.75f).SetDelay(0.8f);
        transform.GetChild(2).gameObject.GetComponent<Image>().DOFade(1, 0.5f).SetDelay(0.8f);
        transform.GetChild(2).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().DOFade(1, 0.5f).SetDelay(0.8f);
    }
}
