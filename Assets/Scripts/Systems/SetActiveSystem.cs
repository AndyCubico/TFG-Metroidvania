using Unity.Burst;
using Unity.Entities;
using UnityEngine;

/// <summary>
/// Handle enabling and disabling (main) GameObjects from systems
/// When SetActiveComponent enabled, system will be executed
/// Needs Monobehaviour with a List of GameObjects. Hardcode the position in list
/// to set it in SetActiveComponent.index.
/// Monobehaviour must have a 
/// Check WeatherInstanceSingleton.cs as a working example
/// </summary>
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

            //var myMono = SystemAPI.ManagedAPI.GetComponentObject<GameObject>(entity);
        }
    }
}
