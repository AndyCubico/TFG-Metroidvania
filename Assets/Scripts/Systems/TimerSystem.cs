using Unity.Burst;
using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(InitializationSystemGroup))]
partial struct TimerSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        state.RequireForUpdate<utils.TimerComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var _timerJob = new TimerJob()
        {
            deltaTime = Time.deltaTime,
            ecb = GetEntityCommandBuffer(ref state),
        };

        var _query = SystemAPI.QueryBuilder()
            .WithAllRW<utils.TimerComponent>()
            .WithDisabled<utils.TimerTriggerComponent>()
            .Build();

        _timerJob.ScheduleParallel(_query);
    }

    private EntityCommandBuffer.ParallelWriter GetEntityCommandBuffer(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        return ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
    }
}

public partial struct TimerJob : IJobEntity
{
    public float deltaTime;
    public EntityCommandBuffer.ParallelWriter ecb;

    public void Execute([EntityIndexInQuery] int entityIndex,
        in Entity entity, ref utils.TimerComponent timerComp)
    {
        timerComp.timer -= deltaTime;

        if (timerComp.timer <= 0)
        {
            ecb.SetComponentEnabled<utils.TimerTriggerComponent>(entityIndex, entity, true);
        }
    }
}