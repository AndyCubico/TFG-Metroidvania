using System;
using UnityEngine;

using Random = UnityEngine.Random;

public enum INTENSITY
{
    LIGHT,
    MEDIUM,
    HEAVY
}

[Serializable]
public class Weather :MonoBehaviour
{
    public WEATHER weather;
    public INTENSITY intensity = INTENSITY.LIGHT;

    [SerializeField] private float duration;  // rl minutes

    [SerializeField] private float minDuration = 3;
    [SerializeField] private float maxDuration = 10;

    public Weather()
    {
    }

    //public Weather(WEATHER w)
    //{
    //    weather = w;
    //    intensity = (INTENSITY)Random.Range((int)INTENSITY.LIGHT, (int)INTENSITY.HEAVY);
    //    duration = Random.Range(minDuration, maxDuration);
    //}

    //public Weather(WEATHER weather, INTENSITY intensity, float duration, float temperatureVariation)
    //{
    //    this.weather = weather;
    //    this.intensity = intensity;
    //    this.duration = duration;
    //}

    #region Getters/Setters
    public float GetDuration() { return duration; }
    public void SetDuration(float d) { duration = d; }
    public void SetRandomDuration() 
    {
        
    }

    public INTENSITY GetIntensity() { return intensity; }
    public void SetIntensity()
    {
        INTENSITY[] temp = new INTENSITY[]
        {
            INTENSITY.LIGHT, INTENSITY.MEDIUM, INTENSITY.HEAVY
        };

        intensity = temp[Random.Range(0, temp.Length)];
    }

    #endregion // Getters/Setters

    // Common awake logic
    public virtual void Awake()
    {
        SetIntensity();
    }

    // Common start logic
    public virtual void Start()
    {
    }

    // Common update logic
    public virtual void Update()
    {

    }

    public void StopWeather()
    {
        this.enabled = false;
    }
}
