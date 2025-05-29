using System;
using UnityEngine;
using UnityEngine.UI;

public static class FadeToBlackEvents
{
    public static Action<float, float> FadeToBlackAction;
}

public class FadeToBlack : MonoBehaviour
{
    enum FADE_STATUS 
    { 
        FADE_IN,
        FADE_OUT,
        WAITING,
        NONE
    }

    Image fadeImage;

    FADE_STATUS fadeStatus;

    float fadeInTime;
    float fadeOutTime;

    float timerToChangeAlpha;

    Color ogColor;

    private void OnEnable()
    {
        FadeToBlackEvents.FadeToBlackAction += FadeToBlackTransition;
    }

    private void OnDisable()
    {
        FadeToBlackEvents.FadeToBlackAction -= FadeToBlackTransition;
    }

    void Start()
    {
        fadeStatus = FADE_STATUS.WAITING;
        timerToChangeAlpha = 0;

        fadeImage = GetComponent<Image>();

        ogColor = fadeImage.color;
    }

    void Update()
    {
        if(fadeStatus == FADE_STATUS.FADE_IN)
        {
            timerToChangeAlpha += Time.deltaTime;

            fadeImage.color = new Color(ogColor.r, ogColor.g, ogColor.b, 1 - ((fadeInTime - timerToChangeAlpha) / fadeInTime));

            if(fadeImage.color.a >= 1)
            {
                fadeStatus = FADE_STATUS.FADE_OUT;
                timerToChangeAlpha = 0;
                fadeImage.color = new Color(ogColor.r, ogColor.g, ogColor.b, 1);
            }
        }

        if (fadeStatus == FADE_STATUS.FADE_OUT)
        {
            timerToChangeAlpha += Time.deltaTime;

            fadeImage.color = new Color(ogColor.r, ogColor.g, ogColor.b, (fadeOutTime - timerToChangeAlpha) / fadeOutTime);

            if (fadeImage.color.a <= 0)
            {
                fadeStatus = FADE_STATUS.WAITING;
                timerToChangeAlpha = 0;
                fadeImage.color = new Color(ogColor.r, ogColor.g, ogColor.b, 0);
            }
        }
    }

    public void FadeToBlackTransition(float timeIn, float timeOut)
    {
        fadeInTime = timeIn;
        fadeOutTime = timeOut;

        fadeStatus = FADE_STATUS.FADE_IN;
        timerToChangeAlpha = 0;
    }
}