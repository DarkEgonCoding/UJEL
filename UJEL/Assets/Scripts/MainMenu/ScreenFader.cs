using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    [SerializeField] public Image fadeImage;
    [SerializeField] public float fadeDuration = 2f;

    public void FadeIn()
    {
        StartCoroutine(FadeInCoroutine());
    }

    public void FadeOut()
    {
        StartCoroutine(FadeOutCoroutine());
    }

    private IEnumerator FadeInCoroutine()
    {
        yield return Fade(0f, 1f); // Fade to black
    }

    private IEnumerator FadeOutCoroutine()
    {
        yield return Fade(1f, 0f); // Fade back in
    }

    public IEnumerator Fade(float fromAlpha, float toAlpha)
    {
        Color color = fadeImage.color;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;
            color.a = Mathf.Lerp(fromAlpha, toAlpha, t);
            fadeImage.color = color;
            yield return null;
        }

        color.a = toAlpha;
        fadeImage.color = color;
    }
}
