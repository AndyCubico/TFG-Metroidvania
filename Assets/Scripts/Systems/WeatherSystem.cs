using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using UnityEngine;

partial struct WeatherStartSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<weather.WeatherComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (
            weatherComp,
            entity)
            in SystemAPI.Query<
                RefRW<weather.WeatherComponent>>()
                .WithAll<utils.TimerTriggerComponent>()
                .WithDisabled<weather.WeatherStartComponent>()
                .WithEntityAccess())
        {
            weatherComp.ValueRW.weather = SetWeightedRandomWeather(weatherComp);

            Debug.Log($"aiubfweiufneoqjf {((int)weatherComp.ValueRW.weather)}");

            switch (weatherComp.ValueRW.weather)
            {
                case WEATHER.RAIN:
                    {
                        Helper.EnableComponent<weather.RainComponent>(ref state, entity, true);
                        var comp = SystemAPI.GetComponentRO<weather.RainComponent>(entity);

                        var activeComp = SystemAPI.GetComponentRW<utils.SetActiveComponent>(entity);
                        activeComp.ValueRW.index = comp.ValueRO.index;
                        activeComp.ValueRW.toEnable = true;
                    }
                    break;
                case WEATHER.SNOW:
                    {
                        Helper.EnableComponent<weather.SnowComponent>(ref state, entity, true);
                        var comp = SystemAPI.GetComponentRO<weather.SnowComponent>(entity);

                        var activeComp = SystemAPI.GetComponentRW<utils.SetActiveComponent>(entity);
                        activeComp.ValueRW.index = comp.ValueRO.index;
                        activeComp.ValueRW.toEnable = true;
                    }
                    break;
                case WEATHER.SUN:
                    {
                        Helper.EnableComponent<weather.SunComponent>(ref state, entity, true);
                        var comp = SystemAPI.GetComponentRO<weather.SunComponent>(entity);

                        var activeComp = SystemAPI.GetComponentRW<utils.SetActiveComponent>(entity);
                        activeComp.ValueRW.index = comp.ValueRO.index;
                        activeComp.ValueRW.toEnable = true;
                    }
                    break;
                case WEATHER.UNKNOWN:
                //[[Fallthrough]]
                default:
                    Debug.Log("[Error] Weather was unknown");
                    return;
            }

            Helper.EnableComponent<utils.SetActiveComponent>(ref state, entity, true);
            Helper.EnableComponent<weather.WeatherStartComponent>(ref state, entity, true);
        }
    }

    // Set random weather based on the probabilities of each biome
    [BurstCompile]
    public WEATHER SetWeightedRandomWeather(RefRW<weather.WeatherComponent> comp)
    {
        // Probabilities sum up to 100
        float random = Random.value * 100;  // Random number 0-100

        float weight = 0;
        float prevWeight = 0;

        using NativeArray<float> rates = new NativeArray<float>(3, Allocator.Temp)
        {
            [0] = comp.ValueRO.rainRate,
            [1] = comp.ValueRO.snowRate,
            [2] = comp.ValueRO.sunRate
        };

        for (int i = 0; i < rates.Length; i++)
        {
            // Set ranges for each weathers
            weight += rates[i];

            if (random > prevWeight && random <= weight)
            {
                Debug.Log($"hrthbdfbed {i}");
                return (WEATHER)i;
            }

            prevWeight = weight;
        }

        return WEATHER.UNKNOWN;
    }
}

partial struct WeatherEndSystem : ISystem
{
    //[BurstCompile]
    //public void OnUpdate(ref SystemState state)
    //{
    //    foreach (var (
    //        weatherComp,
    //        entity)
    //        in SystemAPI.Query<
    //            RefRO<weather.WeatherComponent>>()
    //            .WithPresent<utils.TimerTriggerComponent>()
    //            .WithDisabled<weather.WeatherStartComponent>()
    //            .WithEntityAccess())
    //    {
    //        switch (weatherComp.ValueRO.weather)
    //        {
    //            case WEATHER.RAIN:
    //                {
    //                    Helper.EnableComponent<weather.RainComponent>(ref state, entity, false);
    //                    var comp = SystemAPI.GetComponentRO<weather.RainComponent>(entity);

    //                    var activeComp = SystemAPI.GetComponentRW<utils.SetActiveComponent>(entity);
    //                    activeComp.ValueRW.index = comp.ValueRO.index;
    //                    activeComp.ValueRW.toEnable = false;
    //                }
    //                break;
    //            case WEATHER.SNOW:
    //                {
    //                    Helper.EnableComponent<weather.RainComponent>(ref state, entity, false);
    //                    var comp = SystemAPI.GetComponentRO<weather.RainComponent>(entity);

    //                    var activeComp = SystemAPI.GetComponentRW<utils.SetActiveComponent>(entity);
    //                    activeComp.ValueRW.index = comp.ValueRO.index;
    //                    activeComp.ValueRW.toEnable = false;
    //                }
    //                break;
    //            case WEATHER.SUN:
    //                {
    //                    Helper.EnableComponent<weather.RainComponent>(ref state, entity, false);
    //                    var comp = SystemAPI.GetComponentRO<weather.RainComponent>(entity);

    //                    var activeComp = SystemAPI.GetComponentRW<utils.SetActiveComponent>(entity);
    //                    activeComp.ValueRW.index = comp.ValueRO.index;
    //                    activeComp.ValueRW.toEnable = false;
    //                }
    //                break;
    //            case WEATHER.UNKNOWN:
    //            //[[Fallthrough]]
    //            default:
    //                Debug.Log("[Error] Weather was unknown");
    //                break;
    //        }


    //        Helper.EnableComponent<utils.SetActiveComponent>(ref state, entity, true);
    //        Helper.EnableComponent<weather.WeatherStartComponent>(ref state, entity, false);
    //        Helper.EnableComponent<utils.TimerTriggerComponent>(ref state, entity, false);
    //    }
    //}
}