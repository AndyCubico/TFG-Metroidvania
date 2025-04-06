using Unity.Burst;
using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(InitializationSystemGroup))]
partial struct TimerSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<utils.TimerComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (
            timerComp,
            entity)
            in SystemAPI.Query<
                RefRW<utils.TimerComponent>>()
                .WithDisabled<utils.TimerTriggerComponent>()
                .WithEntityAccess())
        {
            timerComp.ValueRW.timer -= SystemAPI.Time.DeltaTime;

            if (timerComp.ValueRO.timer <= 0)
            {
                if (state.EntityManager.HasComponent<utils.TimerTriggerComponent>(entity))
                {
                    state.EntityManager.SetComponentEnabled<utils.TimerTriggerComponent>(entity, true);
                }
                else
                {
                    Debug.Log("[Error] Entity does not have TimerTriggerComponent");
                }
            }
        }
    }
}