using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

public enum BIOME
{
    DESERT,
    JUNGLE,
    MEADOW,
    MOUNTAIN
}

public class BiomeData
{
    public BIOME Biome { get; set; }
    public float Base { get; set; }
    public float Min { get; set; }
    public float Max { get; set; }
    public float Sun { get; set; }
    public float Rain { get; set; }
    public float Wind { get; set; }
    public float Snow { get; set; }

    public Biome CreateBiome(BIOME biomeType)
    {
        if (Validate())
        {
            Biome biome = new Biome(biomeType, Min, Max, Sun, Rain, Wind, Snow);
            return biome;
        }

        return null;
    }

    // Validate the data before creating a Biome
    public bool Validate()
    {
        return Min <= Max && Sun + Rain + Wind + Snow <= 100; // Validation rules
    }

    // Convert to string
    public string ToCSVString()
    {
        return $"{Biome},{Min},{Max},{Sun},{Rain},{Wind}";
    }
}

[Serializable]
public class Biome
{
    public BIOME biome;

    [SerializeField] private float currentTemperature;
    [SerializeField] private float minTemp = 0.0f;
    [SerializeField] private float maxTemp = 100.0f;

    public Dictionary<WEATHER, float> weatherDictionary = new Dictionary<WEATHER, float>();

    public Biome() { }

    public Biome(BIOME biome, float minTemp, float maxTemp, float sunRate, float rainRate, float windRate, float snowRate)
    {
        this.biome = biome;
        this.minTemp = minTemp;
        this.maxTemp = maxTemp;

        weatherDictionary.Add(WEATHER.SUN, sunRate);
        weatherDictionary.Add(WEATHER.RAIN, rainRate);
        //weatherDictionary.Add(WEATHER.WIND, windRate);
        weatherDictionary.Add(WEATHER.SNOW, snowRate);

        SetRandomTemperature();
    }

    #region 
    public float GetTemperature() { return currentTemperature; }

    public void SetTemperature(float temp) { currentTemperature = temp; }

    public void SetRandomTemperature()
    {
        currentTemperature = Random.Range(minTemp, maxTemp);
    }

    #endregion
}
