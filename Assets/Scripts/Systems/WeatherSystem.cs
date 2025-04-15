using Unity.Burst;
using Unity.Entities;
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
                RefRO<weather.WeatherComponent>>()
                .WithAll<utils.TimerTriggerComponent>()
                .WithDisabled<weather.WeatherStartComponent>()
                .WithEntityAccess())
        {
            switch (weatherComp.ValueRO.weather)
            {
                case WEATHER.RAIN:
                    {
                        //Helper.EnableComponent<weather.RainComponent>(ref state, entity, true);
                        //var comp = SystemAPI.GetComponentRO<weather.RainComponent>(entity);

                        //var activeComp = SystemAPI.GetComponentRW<utils.SetActiveComponent>(entity);
                        //activeComp.ValueRW.isActive = true;
                        //activeComp.ValueRW.entity = comp.ValueRO.entity;

                        //Helper.EnableComponent<utils.SetActiveComponent>(ref state, entity, true);
                        Helper.EnableComponent<utils.SetActiveComponent>(ref state, weatherComp.ValueRO.rainEntity, true);
                        Helper.EnableComponent<weather.WeatherStartComponent>(ref state, entity, true);
                    }
                    break;
                case WEATHER.SNOW:
                    Helper.EnableComponent<weather.SnowComponent>(ref state, entity, true);
                    Helper.EnableComponent<weather.WeatherStartComponent>(ref state, entity, true);
                    break;
                case WEATHER.SUN:
                    Helper.EnableComponent<weather.SunComponent>(ref state, entity, true);
                    Helper.EnableComponent<weather.WeatherStartComponent>(ref state, entity, true);
                    break;
                case WEATHER.UNKNOWN:
                    //Debug.Log("[Error] Weather was unknown");
                    break;
                default:
                    break;
            }
        }
    }
}

partial struct WeatherEndSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        //foreach (var (
        //    weatherComp,
        //    entity)
        //    in SystemAPI.Query<
        //        RefRO<weather.WeatherComponent>>()
        //        .WithPresent<utils.TimerTriggerComponent>()
        //        .WithDisabled<weather.WeatherStartComponent>()
        //        .WithEntityAccess())
        //{
        //    switch (weatherComp.ValueRO.weather)
        //    {
        //        case WEATHER.RAIN:
        //            state.EntityManager.SetComponentEnabled<weather.RainComponent>(entity, true);
        //            state.EntityManager.SetComponentEnabled<weather.WeatherStartComponent>(entity, true);
        //            break;
        //        case WEATHER.SNOW:
        //            state.EntityManager.SetComponentEnabled<weather.SnowComponent>(entity, true);
        //            state.EntityManager.SetComponentEnabled<weather.WeatherStartComponent>(entity, true);
        //            break;
        //        case WEATHER.SUN:
        //            state.EntityManager.SetComponentEnabled<weather.SunComponent>(entity, true);
        //            state.EntityManager.SetComponentEnabled<weather.WeatherStartComponent>(entity, true);
        //            break;
        //        case WEATHER.UNKNOWN:
        //            Debug.Log("[Error] Weather was unknown");
        //            break;
        //        default:
        //            break;
        //    }
        //}
    }
}