using Unity.Entities;
using UnityEngine;

public class TransformAuthoring : MonoBehaviour
{
    public Vector2 m_Position;
    public Vector2 m_Rotation;
    public Vector2 m_Scale; 
    
    public class Baker : Baker<TransformAuthoring>
    {
        public override void Bake(TransformAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new TransformComponent
            {
                m_Position = authoring.m_Position,
                m_Rotation = authoring.m_Rotation,
                m_Scale = authoring.m_Scale
            });
        }
    }
}

public struct TransformComponent : IComponentData
{
    public Vector2 m_Position;
    public Vector2 m_Rotation;
    public Vector2 m_Scale;
}
