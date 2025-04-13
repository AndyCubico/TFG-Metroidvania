using Unity.Entities;
using UnityEngine;

public class WeatherManagerAuthoring : MonoBehaviour
{
    public GameObject rainGO;
    public GameObject snowGO;
    public GameObject sunGO;
    public float duration = 10.0f;

    public class Baker : Baker<WeatherManagerAuthoring>
    {
        public override void Bake(WeatherManagerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.WorldSpace);

            // Singleton Component
            AddComponent(entity, new weather.WeatherComponent
            {
                rainEntity = GetEntity(authoring.rainGO, TransformUsageFlags.Renderable),
                snowEntity = GetEntity(authoring.snowGO, TransformUsageFlags.Renderable),
                sunEntity = GetEntity(authoring.sunGO, TransformUsageFlags.Renderable)
            });

            //AddComponent(entity, new utils.SetActiveComponent { });
            //SetComponentEnabled<utils.SetActiveComponent>(entity, false);

            // Weathers
            //// Rain
            //Entity rainEntity = GetEntity(authoring.rainGO, TransformUsageFlags.Renderable);
            //AddComponent(entity, new weather.RainComponent
            //{
            //    entity = rainEntity
            //});
            //SetComponentEnabled<weather.RainComponent>(entity, false);
            ////AddComponent(rainEntity, new utils.SetActiveComponent { });
            ////SetComponentEnabled<utils.SetActiveComponent>(rainEntity, false);

            ////// Snow
            //Entity snowEntity = GetEntity(authoring.snowGO, TransformUsageFlags.Renderable);
            //AddComponent(entity, new weather.SnowComponent
            //{
            //    entity = snowEntity
            //});
            //SetComponentEnabled<weather.SnowComponent>(entity, false);
            ////AddComponent(snowEntity, new utils.SetActiveComponent { });
            ////SetComponentEnabled<utils.SetActiveComponent>(snowEntity, false);

            ////// Sun
            //Entity sunEntity = GetEntity(authoring.sunGO, TransformUsageFlags.Renderable);
            //AddComponent(entity, new weather.SunComponent
            //{
            //    entity = sunEntity
            //});
            //SetComponentEnabled<weather.SunComponent>(entity, false);
            ////AddComponent(sunEntity, new utils.SetActiveComponent { });
            ////SetComponentEnabled<utils.SetActiveComponent>(sunEntity, false);

            // Weathers
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
