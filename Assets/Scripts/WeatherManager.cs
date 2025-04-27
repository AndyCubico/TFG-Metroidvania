using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

// Manage the Biome type and sets the current weather from the probabilities table
public class WeatherManager : MonoBehaviour
{
    List<BiomeData> biomeDataList;

    public BIOME currentBiome;
    public Biome biome;

    [SerializeField] private Weather currentWeather;

    [SerializeField] private Weather sun;
    [SerializeField] private Weather rain;
    [SerializeField] private Weather wind;
    [SerializeField] private Weather snow;

    public Dictionary<WEATHER, Weather> weathersDictionary = new Dictionary<WEATHER, Weather>();

    // Start is called before the first frame update
    void Start()
    {
        // Create and add biomes to the dictionary from the CSV
        CreateBiomesCSV();

        biome = Globals.biomesDictionary[currentBiome];

        weathersDictionary.Add(WEATHER.SUN, sun);
        weathersDictionary.Add(WEATHER.RAIN, rain);
        //weathersDictionary.Add(WEATHER.WIND, wind);
        weathersDictionary.Add(WEATHER.SNOW, snow);

        SetWeightedRandomWeather();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            StopCurrentWeather();
            SetWeightedRandomWeather();
        }
    }

    // Set random weather based on the probabilities of each biome
    public void SetWeightedRandomWeather()
    {
        // Probabilities sum up to 100
        float random = Random.value * 100;  // Random number 0-100

        float weight = 0;
        float prevWeight = 0;

        for (int i = 0; i < biome.weatherDictionary.Count; i++)
        {
            // Set ranges for each weathers
            weight += biome.weatherDictionary[(WEATHER)i];

            if (random > prevWeight && random <= weight)
            {
                SetWeather((WEATHER)i);  
                break;
            }

            prevWeight = weight;
        }
    }

    public void SetWeather(WEATHER weather)
    {
        currentWeather = weathersDictionary[weather];
        weathersDictionary[weather].gameObject.SetActive(true);
    }

    public void StopCurrentWeather()
    {
        currentWeather.StopWeather();
    }

    #region External

    private void CreateBiomesCSV()
    {
        biomeDataList = ReadCSV.Read<BiomeData>("CSV/WeatherSystem");

        foreach (BiomeData data in biomeDataList)
        {
            Globals.biomesDictionary.Add(data.Biome, data.CreateBiome(data.Biome));
        }
    }

    //public void LoadData(GameData data)
    //{
    //    // biome
    //    this.biome = Globals.biomesDictionary[data.currentBiome];
    //    this.biome.SetTemperature(data.currentTemperature);

    //    // weather
    //    SetWeather(data.currentWeather);
    //}

    //public void SaveData(ref GameData data)
    //{
    //    // biome
    //    data.currentBiome = this.currentBiome;
    //    data.currentTemperature = this.biome.GetTemperature();

    //    // weather
    //    //data.currentWeather = this.currentWeather;
    //    //data.currentIntensity = this.weather.intensity;
    //    //data.currentDuration = this.weather.GetDuration();
    //}

    #endregion // External
}
