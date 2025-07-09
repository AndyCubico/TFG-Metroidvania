using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public static class CameraEvents
{
    public static Action<float, float> eCameraShake;
}

public class CameraShake : MonoBehaviour //https://gist.github.com/ftvs/5822103
{
    // Transform of the camera to shake. Grabs the gameObject's transform
    // if null.
    Transform camTransform;

    // How long the object should shake for.
    float shakeDuration = 0f;

    // Amplitude of the shake. A larger value shakes the camera harder.
    float shakeAmount = 0.7f;
    public float decreaseFactor = 1.0f;

    Vector3 originalPos;

    void Awake()
    {
        if (camTransform == null)
        {
            camTransform = GetComponent(typeof(Transform)) as Transform;
        }
    }

    private void OnEnable()
    {
        CameraEvents.eCameraShake += Shake;
    }

    private void OnDisable()
    {
        CameraEvents.eCameraShake -= Shake;
    }

    void LateUpdate()
    {
        if (shakeDuration > 0)
        {
            camTransform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;

            shakeDuration -= Time.deltaTime * decreaseFactor;
        }
        else
        {
            shakeDuration = 0f;
            //camTransform.localPosition = originalPos;
        }
    }

    public void Shake(float duration, float strength)
    {
        originalPos = this.transform.localPosition;
        shakeDuration = duration;
        shakeAmount = strength;
    }
}
