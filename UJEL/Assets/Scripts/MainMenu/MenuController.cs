using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] public GameObject MainMenu;
    [SerializeField] public GameObject Screen;
    [SerializeField] public GameObject Video;
    [SerializeField] public GameObject PresentsScreen;
    [SerializeField] private TextMeshProUGUI title;
    private TextMeshProUGUI[] PresentTexts;

    private void Awake()
    {
        PresentTexts = PresentsScreen.GetComponentsInChildren<TextMeshProUGUI>(true); // include inactive ones
    }

    // Start is called before the first frame update
    void Start()
    {
        MainMenu.SetActive(false);
        PresentsScreen.SetActive(false);
        Screen.SetActive(false);
        Video.SetActive(false);
        StartCoroutine(StartMenuAnimation());
    }

    public IEnumerator StartMenuAnimation()
    {
        yield return new WaitForSeconds(1f);
        foreach (var text in PresentTexts)
        {
            text.alpha = 0;
        }

        PresentsScreen.SetActive(true);

        foreach (var text in PresentTexts)
        {
            yield return StartCoroutine(FadeTextToFullAlpha(3f, text));
            yield return new WaitForSeconds(1f);
            yield return StartCoroutine(FadeTextToZeroAlpha(1f, text));
            yield return new WaitForSeconds(1f);
        }

        yield return new WaitForSeconds(0.75f);

        title.alpha = 0;
        MainMenu.SetActive(true);
        Screen.SetActive(true);
        Video.SetActive(true);
        yield return StartCoroutine(FadeTextToFullAlpha(1f, title));
    }

    public IEnumerator FadeTextToFullAlpha(float duration, TextMeshProUGUI text)
    {
        Color color = text.color;
        float time = 0f;

        while (time < duration)
        {
            float t = time / duration;
            float easedAlpha = t * t; // Quadratic easing-in
            color.a = Mathf.Clamp01(easedAlpha);
            text.color = color;

            time += Time.deltaTime;
            yield return null;
        }

        color.a = 1f;
        text.color = color;
    }

    public IEnumerator FadeTextToZeroAlpha(float duration, TextMeshProUGUI text)
    {
        Color color = text.color;
        float time = 0f;

        while (time < duration)
        {
            float t = time / duration;
            float easedAlpha = 1 - (t * t); // Quadratic easing-in (in reverse)
            color.a = Mathf.Clamp01(easedAlpha);
            text.color = color;

            time += Time.deltaTime;
            yield return null;
        }

        color.a = 0f;
        text.color = color;
    }
}
