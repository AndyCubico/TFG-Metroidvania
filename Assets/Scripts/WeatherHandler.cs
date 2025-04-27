using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherHandler : MonoBehaviour
{
    [SerializeField] private WeatherManager weather_manager;
    public Weather weather;

    [SerializeField] private ParticleSystem mainParticles;
    [SerializeField] private AudioSource lightAudio;
    [SerializeField] private AudioSource mediumAudio;
    [SerializeField] private AudioSource heavyAudio;

    // Start is called before the first frame update
    void Start()
    {
        //weather = new Weather(weather_manager.currentWeather);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
