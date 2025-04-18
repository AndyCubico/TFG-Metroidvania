using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class WeatherManagerAuthoring : MonoBehaviour
{
    public int setIndex;
    public int rainIndex;
    public int snowIndex;
    public int sunIndex;

    public float duration = 10.0f;

    public int rainRate;
    public int snowRate;
    public int sunRate;

    public class Baker : Baker<WeatherManagerAuthoring>
    {
        public override void Bake(WeatherManagerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.WorldSpace);

            // Singleton Component
            AddComponent(entity, new weather.WeatherComponent
            {
                duration = authoring.duration,

                rainRate = authoring.rainRate,
                snowRate = authoring.snowRate,
                sunRate = authoring.sunRate,
            });

            Helper.AddComponentWithDisabled(this, entity, new utils.SetActiveComponent { });

            // Weather
            //// Rain
            Helper.AddComponentWithDisabled(this, entity, new weather.RainComponent
            {
                setIndex = authoring.setIndex,
                index = authoring.rainIndex,
            });

            //// Snow
            Helper.AddComponentWithDisabled(this, entity, new weather.SnowComponent
            {
                setIndex = authoring.setIndex,
                index = authoring.snowIndex,
            });

            //// Sun
            Helper.AddComponentWithDisabled(this, entity, new weather.SunComponent
            {
                setIndex = authoring.setIndex,
                index = authoring.sunIndex,
            });

            // Weather specifics
            AddComponent(entity, new weather.WeatherStartComponent { });
            SetComponentEnabled<weather.WeatherStartComponent>(entity, false);

            // Timer
            AddComponent(entity, new utils.TimerComponent
            {
                targetDuration = authoring.duration,
                timer = authoring.duration,
            });

            AddComponent(entity, new utils.TimerTriggerComponent { });
            SetComponentEnabled<utils.TimerTriggerComponent>(entity, false);
        }
    }
}
