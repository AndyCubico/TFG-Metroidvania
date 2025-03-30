using Unity.Entities;
using UnityEngine;

public class SpeedAuthoring : MonoBehaviour
{
    public Vector2 m_Speed;
    public class Baker : Baker<SpeedAuthoring>
    {
        public override void Bake(SpeedAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic); //Changed from WorldSpace to Dynamic
            AddComponent(entity, new SpeedComponent
            {
                m_Value = authoring.m_Speed
            });
        }
    }
}

public struct SpeedComponent : IComponentData
{
    public Vector2 m_Value;
}