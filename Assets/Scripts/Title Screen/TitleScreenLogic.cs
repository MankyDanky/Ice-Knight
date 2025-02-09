using UnityEngine;
using System.Collections;
public class TitleScreenLogic : MonoBehaviour
{
    [SerializeField] GameObject Title;
    [SerializeField] GameObject FadeIn;
    [SerializeField] private float TitleDelay = 2;
    [SerializeField] private float FadeinDelay = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Invoke("FadeInEffect", FadeinDelay);

        Invoke("TitleScreenAnimation", TitleDelay);

    }

    void FadeInEffect()
    {
        FadeIn.GetComponent<FadeIn>().StartFadeIn();
    }
    void TitleScreenAnimation()
    {
        Title.GetComponent<TitleScreenAnimation>().RevealTitle();

    }
}
