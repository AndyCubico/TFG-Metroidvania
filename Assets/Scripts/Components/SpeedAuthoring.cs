using Unity.Entities;
using UnityEngine;

public class SpeedAuthoring : MonoBehaviour
{
    public Vector2 speed;

    public class Baker : Baker<SpeedAuthoring>
    {
        public override void Bake(SpeedAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new SpeedComponent
            {
                value = authoring.speed
            });
        }
    }
}

public struct SpeedComponent : IComponentData
{
    public Vector2 value;
}