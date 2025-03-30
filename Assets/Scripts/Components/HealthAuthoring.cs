using Unity.Entities;
using UnityEngine;

public class HealthAuthoring : MonoBehaviour
{
    public int m_Health;

    public class Baker : Baker<HealthAuthoring>
    {
        public override void Bake(HealthAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new HealthComponent
            {
                m_Value = authoring.m_Health
            });
        }
    }
}

public struct HealthComponent : IComponentData
{
    public int m_Value;
}