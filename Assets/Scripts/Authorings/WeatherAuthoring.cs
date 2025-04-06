using Unity.Entities;
using UnityEngine;

public class WeatherAuthoring : MonoBehaviour
{
    public GameObject rainGO;
    public GameObject snowGO;
    public GameObject sunGO;
    public float duration = 10.0f;

    public class Baker : Baker<WeatherAuthoring>
    {
        public override void Bake(WeatherAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.WorldSpace);

            // Singleton Component
            AddComponent(entity, new weather.WeatherComponent { /*weather = WEATHER.UNKNOWN*/ });

            // Weathers
            AddComponent(entity, new weather.RainComponent
            {
                entity = GetEntity(authoring.rainGO, TransformUsageFlags.Renderable)
            });
            SetComponentEnabled<weather.RainComponent>(entity, false);

            AddComponent(entity, new weather.SnowComponent
            {
                entity = GetEntity(authoring.snowGO, TransformUsageFlags.Renderable)
            });
            SetComponentEnabled<weather.SnowComponent>(entity, false);

            AddComponent(entity, new weather.SunComponent
            {
                entity = GetEntity(authoring.sunGO, TransformUsageFlags.Renderable)
            });
            SetComponentEnabled<weather.SunComponent>(entity, false);

            AddComponent(entity, new weather.WeatherStartComponent { });
            SetComponentEnabled<weather.WeatherStartComponent>(entity, false);

            AddComponent(entity, new weather.WeatherEndComponent { });
            SetComponentEnabled<weather.WeatherEndComponent>(entity, false);

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
