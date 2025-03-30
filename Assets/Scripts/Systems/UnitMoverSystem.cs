using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Physics;

partial struct UnitMoverSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
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
        foreach ((
            RefRW<LocalTransform> localTransform,
            RefRO<SpeedComponent> moveSpeed,
            RefRW<PhysicsVelocity> physicsVelocity)
            in SystemAPI.Query<
                RefRW<LocalTransform>,
                RefRO<SpeedComponent>,
                RefRW<PhysicsVelocity>>())
        {
            physicsVelocity.ValueRW.Linear.x = moveSpeed.ValueRO.m_Value.x;
            physicsVelocity.ValueRW.Linear.y = moveSpeed.ValueRO.m_Value.y;
            physicsVelocity.ValueRW.Angular = float3.zero;
        }
    }
}
