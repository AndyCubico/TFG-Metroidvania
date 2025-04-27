using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weather_Rain : Weather
{
    [Header ("Rain")]
    [SerializeField] private WeatherManager weather_manager;

    [SerializeField] private ParticleSystem mainParticles;
    [SerializeField] private AudioSource lightAudio;
    [SerializeField] private AudioSource mediumAudio;
    [SerializeField] private AudioSource heavyAudio;

    override public void Awake()
    {
        base.Awake();
    }

    // Start is called before the first frame update
    override public void Start()
    {

    }

    // Update is called once per frame
    override public void Update()
    {

    }

    private void SetParticlesRate()
    { }
}