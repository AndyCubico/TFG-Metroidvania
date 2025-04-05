using System.Diagnostics;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

partial struct UnitMoverSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        UnitMoverJob unitMoverJob = new UnitMoverJob
        {
            deltaTime = SystemAPI.Time.DeltaTime,
        };

        unitMoverJob.ScheduleParallel();


        // Both examples without jobs

        // No physics
        //foreach ((
        //    RefRW<LocalTransform> localTransform,
        //    RefRO<SpeedComponent> moveSpeed)
        //    in SystemAPI.Query<
        //        RefRW<LocalTransform>,
        //        RefRO<SpeedComponent>>())
        //{
        //    localTransform.ValueRW.Position = localTransform.ValueRO.Position + 
        //        new float3(moveSpeed.ValueRO.m_Value.x, moveSpeed.ValueRO.m_Value.y, 0 * SystemAPI.Time.DeltaTime);
        //}

        // Physics
        //foreach (var (
        //    localTransform,
        //    moveSpeed,
        //    physicsVelocity)
        //    in SystemAPI.Query<
        //        RefRW<LocalTransform>,
        //        RefRO<SpeedComponent>,
        //        RefRW<PhysicsVelocity>>())
        //{
        //    physicsVelocity.ValueRW.Linear.x = moveSpeed.ValueRO.value.x;
        //    physicsVelocity.ValueRW.Linear.y = moveSpeed.ValueRO.value.y;
        //    physicsVelocity.ValueRW.Angular = float3.zero;
        //}
    }

    [BurstCompile]
    public partial struct UnitMoverJob : IJobEntity
    {
        public float deltaTime; // Example of how to use external variables

        public void Execute(ref LocalTransform localTransform, in movement.SpeedComponent moveSpeed, ref PhysicsVelocity physicsVelocity)
        {
            physicsVelocity.Linear.x = moveSpeed.value.x;
            physicsVelocity.Linear.y = moveSpeed.value.y;
            physicsVelocity.Angular = float3.zero;

            Debug.WriteLine(deltaTime);
        }
    }
}
