using Unity.Entities;

namespace utils
{
    public struct TimerComponent : QG_IComponent
    {
        public float targetDuration;    // Set only once
        public float timer;         // Handles the countdown
        public Entity entityHolder;
        public ComponentType componentType;
    }

    public struct TimerTriggerComponent : QG_IEnableComponent
    { }
}