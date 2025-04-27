using System;
using Unity.Entities;
using UnityEngine;

public class UuidAuthoring : MonoBehaviour
{
    public Guid guid;

    public class Baker : Baker<UuidAuthoring>
    {
        public override void Bake(UuidAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new UuidComponent
            {
                guid = authoring.guid
            });
        }
    }
}

public struct UuidComponent : QG_IEnableComponent
{
    public Guid guid;
}