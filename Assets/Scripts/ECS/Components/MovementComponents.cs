using Unity.Mathematics;

namespace movement
{
    public struct TransformComponent : QG_IEnableComponent
    {
        public float2 position;
        public float2 rotation;
        public float2 scale;
    }

    public struct SpeedComponent : QG_IEnableComponent
    {
        public float2 value;
    }
}