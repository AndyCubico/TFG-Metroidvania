using Unity.Entities;
using UnityEngine;
public enum WEATHER
{
    SUN,
    RAIN,
    SNOW,

    UNKNOWN
}

namespace weather
{
    public struct WeatherComponent : IComponentData
    {
        public WEATHER weather;
        public float duration;  // rl minutes
    }

    public struct IsRainingComponent : IComponentData
    {
    }

    public struct IsSnowingComponent : IComponentData
    {
    }

    public struct IsSunComponent : IComponentData
    {
    }
}