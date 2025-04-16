using System.Collections.Generic;
using UnityEngine;

public class WeatherInstanceSingleton : MonoBehaviour
{
    public static WeatherInstanceSingleton Instance { get; private set; }

    public void Awake()
    {
        Instance = this;
    }

    [Tooltip("Must add from script")]
    public List<GameObject> gameObjectsList;
}