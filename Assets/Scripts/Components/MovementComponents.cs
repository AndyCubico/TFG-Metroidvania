using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace movement
{
    public struct TransformComponent : IComponentData
    {
        public float2 position;
        public float2 rotation;
        public float2 scale;
    }

    public struct SpeedComponent : IComponentData
    {
        public float2 value;
    }
}