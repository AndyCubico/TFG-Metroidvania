public enum WEATHER
{
    SUN,
    RAIN,
    SNOW,

    UNKNOWN
}

namespace weather
{
    public struct WeatherComponent : QG_IEnableComponent
    {
        public WEATHER weather;
        public float duration;  // rl minutes
    }

    public struct RainComponent : QG_IEnableComponent
    {
    }

    public struct SnowComponent : QG_IEnableComponent
    {
    }

    public struct SunComponent : QG_IEnableComponent
    {
    }
}