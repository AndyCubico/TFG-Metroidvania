using System;
using Unity.Entities;
using UnityEngine;

public class UuidAuthoring : MonoBehaviour
{
    public Guid m_Guid;

    public class Baker : Baker<UuidAuthoring>
    {
        public override void Bake(UuidAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new UuidComponent
            {
                m_Guid = authoring.m_Guid
            });
        }
    }
}

public struct UuidComponent : IComponentData
{
    public Guid m_Guid;
}