using System.Collections.Generic;
using Unity.Entities;

public enum WEATHER
{
    RAIN,
    SNOW,
    SUN,

    UNKNOWN
}

namespace weather
{
    public struct WeatherComponent : QG_IEnableComponent
    {
        public WEATHER weather;
        public float duration;  // rl minutes

        public int rainRate;
        public int snowRate;
        public int sunRate;
    }

    public struct RainComponent : QG_IEnableComponent
    {
        public int index;
    }

    public struct SnowComponent : QG_IEnableComponent
    {
        public int index;
    }

    public struct SunComponent : QG_IEnableComponent
    {
        public int index;
    }

    public struct WeatherStartComponent : QG_IEnableComponent
    {
    }

    public struct WeatherEndComponent : QG_IEnableComponent
    {
    }
}