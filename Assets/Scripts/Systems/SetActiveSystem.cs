using Unity.Burst;
using Unity.Entities;
using UnityEngine;

partial struct SetActiveSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<utils.SetActiveComponent>();
    }

    public void OnUpdate(ref SystemState state)
    {
        foreach (var (activeComp, entity)
            in SystemAPI.Query<RefRO<utils.SetActiveComponent>>()
            .WithEntityAccess())
        {
            bool toEnable = activeComp.ValueRO.toEnable;
            WeatherInstanceSingleton.Instance.gameObjectsList[activeComp.ValueRO.index].SetActive(toEnable);

            Debug.Log($"Enabling {WeatherInstanceSingleton.Instance.gameObjectsList[activeComp.ValueRO.index].name}: {toEnable}");
            Helper.EnableComponent<utils.SetActiveComponent>(ref state, entity, false);
        }
    }
}
