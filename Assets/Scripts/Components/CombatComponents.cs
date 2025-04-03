using Unity.Entities;
using UnityEngine;

namespace combat
{
    public struct HealthComponent : IComponentData
    {
        public int value;
    }
}