using Unity.Burst;
using Unity.Entities;
using UnityEngine;

partial struct SetActiveSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<utils.SetActiveComponent>();
    }

    public void OnUpdate(ref SystemState state)
    {
        foreach (var activeComp
            in SystemAPI.Query<RefRO<utils.SetActiveComponent>>())
        {
            Debug.Log("adqefef");
            var gameObject = state.EntityManager.GetComponentObject<GameObject>(activeComp.ValueRO.entity);
            gameObject.SetActive(activeComp.ValueRO.isActive);
        }
    }
}
