using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using DG.Tweening;

public class PlayHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public RawImage fadeElement;
    public AudioSource music;
    public AudioSource hoverSFX;

    public List<AudioSource> SFXToPlayOnClick = new List<AudioSource>();

    IEnumerator startGame() {
        fadeElement.DOFade(1, 4f);
        music.DOFade(0, 2f);

        foreach (AudioSource clip in SFXToPlayOnClick)
        {
            clip.Play();
        }

        yield return new WaitForSeconds(6f);
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    public void OnPointerClick(PointerEventData pointerEventData) {
        StartCoroutine(startGame());
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
