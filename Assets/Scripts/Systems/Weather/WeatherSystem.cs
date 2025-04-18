//using Unity.Burst;
//using Unity.Collections;
//using Unity.Entities;
//using UnityEngine;

//partial struct WeatherEndSystem : ISystem
//{
//    [BurstCompile]
//    public void OnUpdate(ref SystemState state)
//    {
//        foreach (var (
//            weatherComp,
//            entity)
//            in SystemAPI.Query<
//                RefRO<weather.WeatherComponent>>()
//                .WithPresent<utils.TimerTriggerComponent>()
//                .WithDisabled<weather.WeatherStartComponent>()
//                .WithEntityAccess())
//        {
//            switch (weatherComp.ValueRO.weather)
//            {
//                case WEATHER.RAIN:
//                    {
//                        Helper.EnableComponent<weather.RainComponent>(ref state, entity, false);
//                        var comp = SystemAPI.GetComponentRO<weather.RainComponent>(entity);

//                        var activeComp = SystemAPI.GetComponentRW<utils.SetActiveComponent>(entity);
//                        activeComp.ValueRW.index = comp.ValueRO.index;
//                        activeComp.ValueRW.toEnable = false;
//                    }
//                    break;
//                case WEATHER.SNOW:
//                    {
//                        Helper.EnableComponent<weather.RainComponent>(ref state, entity, false);
//                        var comp = SystemAPI.GetComponentRO<weather.RainComponent>(entity);

//                        var activeComp = SystemAPI.GetComponentRW<utils.SetActiveComponent>(entity);
//                        activeComp.ValueRW.index = comp.ValueRO.index;
//                        activeComp.ValueRW.toEnable = false;
//                    }
//                    break;
//                case WEATHER.SUN:
//                    {
//                        Helper.EnableComponent<weather.RainComponent>(ref state, entity, false);
//                        var comp = SystemAPI.GetComponentRO<weather.RainComponent>(entity);

//                        var activeComp = SystemAPI.GetComponentRW<utils.SetActiveComponent>(entity);
//                        activeComp.ValueRW.index = comp.ValueRO.index;
//                        activeComp.ValueRW.toEnable = false;
//                    }
//                    break;
//                case WEATHER.UNKNOWN:
//                //[[Fallthrough]]
//                default:
//                    Debug.Log("[Error] Weather was unknown");
//                    break;
//            }

//            Helper.EnableComponent<utils.SetActiveComponent>(ref state, entity, true);
//            Helper.EnableComponent<weather.WeatherStartComponent>(ref state, entity, false);
//            Helper.EnableComponent<weather.WeatherEndComponent>(ref state, entity, true);
//            Helper.EnableComponent<utils.TimerTriggerComponent>(ref state, entity, false);
//        }
//    }
//}