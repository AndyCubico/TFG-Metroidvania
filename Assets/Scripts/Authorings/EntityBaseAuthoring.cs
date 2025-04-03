using combat;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class EntityBaseAuthoring : MonoBehaviour
{
    [Header("Movement")]
    public float2 position;
    public float2 rotation;
    public float2 scale;

    public float2 speed;

    [Header("Combat")]
    public int health;

    public class Baker : Baker<EntityBaseAuthoring>
    {
        public override void Bake(EntityBaseAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new movement.TransformComponent
            {
                position = authoring.position,
                rotation = authoring.rotation,
                scale = authoring.scale
            });

            AddComponent(entity, new movement.SpeedComponent
            {
                value = authoring.speed
            });

            AddComponent(entity, new combat.HealthComponent
            {
                value = authoring.health
            });
        }
    }
}
