using Unity.Burst;
using Unity.Entities;
using UnityEngine;

/// <summary>
/// When the timer trigger reaches 0, set the components so the system resets
/// 
/// If it has WeatherStartComponent --> it has started
/// && If it the timer has triggered --> it has ended
/// </summary>
[UpdateAfter(typeof(WeatherStartSystem))]
partial struct WeatherEndSystem : ISystem
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
                RefRO<weather.WeatherComponent>>()
                .WithAll<utils.TimerTriggerComponent
                , weather.WeatherStartComponent>()
                .WithEntityAccess())
        {
            switch (weatherComp.ValueRO.weather)
            {
                case WEATHER.RAIN:
                    {
                        Helper.EnableComponent<weather.RainComponent>(ref state, entity, false);
                        var comp = SystemAPI.GetComponentRO<weather.RainComponent>(entity);

                        var activeComp = SystemAPI.GetComponentRW<utils.SetActiveComponent>(entity);
                        activeComp.ValueRW.setIndex = comp.ValueRO.setIndex;
                        activeComp.ValueRW.index = comp.ValueRO.index;
                        activeComp.ValueRW.toEnable = false;
                    }
                    break;
                case WEATHER.SNOW:
                    {
                        Helper.EnableComponent<weather.SnowComponent>(ref state, entity, false);
                        var comp = SystemAPI.GetComponentRO<weather.SnowComponent>(entity);

                        var activeComp = SystemAPI.GetComponentRW<utils.SetActiveComponent>(entity);
                        activeComp.ValueRW.setIndex = comp.ValueRO.setIndex;
                        activeComp.ValueRW.index = comp.ValueRO.index;
                        activeComp.ValueRW.toEnable = false;
                    }
                    break;
                case WEATHER.SUN:
                    {
                        Helper.EnableComponent<weather.SunComponent>(ref state, entity, false);
                        var comp = SystemAPI.GetComponentRO<weather.SunComponent>(entity);

                        var activeComp = SystemAPI.GetComponentRW<utils.SetActiveComponent>(entity);
                        activeComp.ValueRW.setIndex = comp.ValueRO.setIndex;
                        activeComp.ValueRW.index = comp.ValueRO.index;
                        activeComp.ValueRW.toEnable = false;
                    }
                    break;
                case WEATHER.UNKNOWN:
                //[[Fallthrough]]
                default:
                    Debug.Log("[Error] Weather was unknown");
                    break;
            }

            Helper.EnableComponent<utils.SetActiveComponent>(ref state, entity, true);
            Helper.EnableComponent<weather.WeatherStartComponent>(ref state, entity, false);
        }
    }
}