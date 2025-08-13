using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weather_Sun : Weather
{
    [Header("Sun")]
    [SerializeField] private WeatherManagerOld weather_manager;

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
