using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weather_Wind: Weather
{
    [Header("Wind")]
    [SerializeField] private WeatherManager weather_manager;

    override public void Awake()
    {
        base.Awake();
    }

    // Start is called before the first frame update
    override public void Start()
    {
        base.Start();

    }

    // Update is called once per frame
    override public void Update()
    {
        base.Update();

    }
}
