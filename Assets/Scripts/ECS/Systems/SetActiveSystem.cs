using Unity.Burst;
using Unity.Entities;
using UnityEngine;

/// <summary>
/// Handle enabling and disabling (main) GameObjects from systems
/// When SetActiveComponent enabled, system will be executed
/// Needs GameObject (ECS manager) with SetActiveInstanceSingleton.cs in Scene.
/// Create a List from inspector and Add the GameObject(s) in the sublist.
/// Hardcode the position in gameObjectsList: SetActiveComponent.setIndex
/// Hardcode the position in subList (List in gameObjectsList): SetActiveComponent.index
/// Example:
/// Check WeatherManagerAuthoring.cs Ln-39 --> setting the component to store the indices
/// Check WeatherStartSystem.cs Ln-38 --> setting the SetActiveComponent parameters
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
            SetActiveInstanceSingleton.Instance.gameObjectsList[activeComp.ValueRO.setIndex][activeComp.ValueRO.index].SetActive(toEnable);

            Debug.Log($"Enabling {SetActiveInstanceSingleton.Instance.gameObjectsList[activeComp.ValueRO.setIndex][activeComp.ValueRO.index].name}: {toEnable}");
            Helper.EnableComponent<utils.SetActiveComponent>(ref state, entity, false);
        }
    }
}
