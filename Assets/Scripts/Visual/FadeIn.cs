using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class FadeIn : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 1f;

    void Start()
    {
        StartCoroutine(Fade());
    }

    private IEnumerator Fade()
    {
        float time = 0f;
        Color color = fadeImage.color;
        color.a = 1f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            color.a = 1f - (time / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        color.a = 0f;

        fadeImage.color = color;

    }
}
