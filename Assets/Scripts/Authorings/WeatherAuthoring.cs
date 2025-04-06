using Unity.Entities;
using UnityEditor;
using UnityEngine;
using utils;

public class WeatherAuthoring : MonoBehaviour
{
    public float duration = 10.0f;

    public class Baker : Baker<WeatherAuthoring>
    {
        public override void Bake(WeatherAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.WorldSpace);

            // Singleton Component
            AddComponent(entity, new weather.WeatherComponent { });
            AddComponent(entity, new utils.TimerComponent
            {
                targetDuration = authoring.duration,
                timer = authoring.duration,
            });

            AddComponent(entity, new weather.RainComponent { });
            AddComponent(entity, new weather.SnowComponent { });
            AddComponent(entity, new weather.SunComponent { });

            AddComponent(entity, new utils.TimerTriggerComponent { });
            SetComponentEnabled<TimerTriggerComponent>(entity, false);
        }
    }
}
