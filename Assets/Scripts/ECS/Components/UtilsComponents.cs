using Unity.Entities;

namespace utils
{
    // If added, change the entity's active state to isActive
    public struct SetActiveComponent : QG_IEnableComponent
    {
        public bool toEnable;
        public int setIndex;    // Index of the List (in SetActiveSingleton.cs)
        public int index;
    }

    public struct TimerComponent : QG_IComponent
    {
        public float targetDuration;    // Set only once
        public float timer;         // Handles the countdown
    }

    // Component that gets enabled to the entity with the timer when the timer is off
    // Disable component to reset the counter
    // Must be added and disabled in the Authoring
    public struct TimerTriggerComponent : QG_IEnableComponent
    { }
}