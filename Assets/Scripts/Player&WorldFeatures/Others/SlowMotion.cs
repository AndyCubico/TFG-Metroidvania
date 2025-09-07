using System;
using UnityEngine;
public static class SlowMotionEffect
{
    public static Action<float, float> eSlowMotion;
    public static Action<float, float> eSlowMotionOn;
    public static Action eSlowMotionOff;
}

public class SlowMotion : MonoBehaviour
{
    float m_slowdownFactor = 0.05f;
    float m_slowdownLength = 2f;

    bool returnToNormalState = true;

    private void OnEnable()
    {
        SlowMotionEffect.eSlowMotion += DoSlowMotion;
        SlowMotionEffect.eSlowMotionOn += SlowMotionOn;
        SlowMotionEffect.eSlowMotionOff += SlowMotionOff;
    }

    private void OnDisable()
    {
        SlowMotionEffect.eSlowMotion -= DoSlowMotion;
    }

    // Update is called once per frame
    void Update()
    {
        if (returnToNormalState)
        {
            Time.timeScale += (1f / m_slowdownLength) * Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Clamp01(Time.timeScale);

            if (Time.timeScale == 1)
            {
                //Time.fixedDeltaTime = Time.timeScale * .02f;
            }
        }
    }

    public void DoSlowMotion(float slowTime, float slowFactor)
    {
        returnToNormalState = true;
        m_slowdownLength = slowTime;
        m_slowdownFactor = slowFactor;

        Time.timeScale = m_slowdownFactor;
        //Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }

    private void SlowMotionOn(float slowTime, float slowFactor)
    {
        returnToNormalState = false;
        m_slowdownLength = slowTime;
        m_slowdownFactor = slowFactor;

        Time.timeScale = slowTime;
    }

    private void SlowMotionOff()
    {
        returnToNormalState = true;
    }
}
