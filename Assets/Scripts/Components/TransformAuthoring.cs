using Unity.Entities;
using UnityEngine;

public class TransformAuthoring : MonoBehaviour
{
    public Vector2 position;
    public Vector2 rotation;
    public Vector2 scale; 
    
    public class Baker : Baker<TransformAuthoring>
    {
        public override void Bake(TransformAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new TransformComponent
            {
                position = authoring.position,
                rotation = authoring.rotation,
                scale = authoring.scale
            });
        }
    }
}

public struct TransformComponent : IComponentData
{
    public Vector2 position;
    public Vector2 rotation;
    public Vector2 scale;
}
