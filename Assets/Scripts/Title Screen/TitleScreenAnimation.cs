using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TitleScreenAnimation : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 0.5f;    // How long each fade-in lasts
    [SerializeField] private float nextLetterTimer = 0.1f;
    [SerializeField] private GameObject[] img;            // Letter objects

    public void RevealTitle()
    {
        foreach (GameObject letter in img)
        {
            Image renderer = letter.GetComponent<Image>();
            Color color = renderer.color;
            color.a = 0f; // Start with alpha 0
            renderer.color = color;
            renderer.enabled = true;
        }

        StartCoroutine(FadeInAllLetters());
    }

    private IEnumerator FadeInAllLetters()
    {
        foreach (GameObject letter in img)
        {
            Image renderer = letter.GetComponent<Image>();
            StartCoroutine(FadeIn(renderer, fadeDuration)); // Start fading all letters in parallel
            yield return new WaitForSeconds(nextLetterTimer);
        }
        yield return null;
    }

    private IEnumerator FadeIn(Image img, float duration)
    {
        float elapsedTime = 0f;
        Color color = img.color;
        color.a = 0f;
        img.color = color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, elapsedTime / duration);
            img.color = color;
            yield return null;
        }

        color.a = 1f;
        img.color = color;
    }
}
