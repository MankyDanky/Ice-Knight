using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeIn : MonoBehaviour
{
    public Image fadeImage;  // Assign in Inspector (the Panel’s Image component)
    public float fadeDuration = 2f;  // Duration of fade effect

    private void Start()
    {
        GetComponent<Image>().enabled = true;

    }

    public void StartFadeIn()
    {
        StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        float elapsedTime = 0f;
        Color startColor = fadeImage.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            fadeImage.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        fadeImage.color = new Color(startColor.r, startColor.g, startColor.b, 0); // Ensure it's fully transparent
    }
}