using Unity.Burst;
using Unity.Entities;

partial struct WeatherSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var weatherComp
            in SystemAPI.Query<
                RefRO<weather.WeatherComponent>>())
        {
            switch (weatherComp.ValueRO.weather)
            {
                case WEATHER.SUN:

                    break;
                case WEATHER.RAIN:
                    break;
                case WEATHER.SNOW:
                    break;
                case WEATHER.UNKNOWN:
                    break;
                default:
                    break;
            }
        }
    }
}