using Unity.Entities;
using UnityEngine;

public class WeatherAuthoring : MonoBehaviour
{
    public GameObject weatherGO;
    public WEATHER weatherType;

    public class Baker : Baker<WeatherAuthoring>
    {
        public override void Bake(WeatherAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.WorldSpace);

            AddComponent(entity, new weather.RainComponent
            {
                entity = entity
            });

            AddComponent(entity, new utils.SetActiveComponent
            {
                entity = entity
            });
            SetComponentEnabled<utils.SetActiveComponent>(entity, false);

            AddComponentObject(entity, authoring.weatherGO.gameObject);

            //switch (authoring.weatherType)
            //{
            //    case WEATHER.RAIN:
            //        {
            //            Entity temp = GetEntity(authoring.weatherGO, TransformUsageFlags.Renderable);
            //            AddComponent(entity, new weather.RainComponent
            //            {
            //                entity = GetEntity(authoring.gameObject, TransformUsageFlags.Renderable)
            //            });

            //            AddComponent(entity, new utils.SetActiveComponent
            //            {
            //                entity = GetEntity(authoring.gameObject, TransformUsageFlags.Renderable)
            //            });
            //            SetComponentEnabled<utils.SetActiveComponent>(entity, false);

            //            AddComponentObject(entity, authoring.gameObject);
            //        }
            //        break;

            //    case WEATHER.SNOW:
            //        {
            //            AddComponent(entity, new weather.SnowComponent
            //            {
            //                entity = GetEntity(authoring.weatherGO, TransformUsageFlags.Renderable)
            //            });
            //        }
            //        break;

            //    case WEATHER.SUN:
            //        {
            //            AddComponent(entity, new weather.SunComponent
            //            {
            //                entity = GetEntity(authoring.weatherGO, TransformUsageFlags.Renderable)
            //            });
            //        }
            //        break;

            //    case WEATHER.UNKNOWN:
            //        {
            //            Debug.Log("[ERROR] Weather not set");
            //        }
            //        break;
            //}
        }
    }
}
