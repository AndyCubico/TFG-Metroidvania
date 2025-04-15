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
        foreach (var (activeComp,
            entity)
            in SystemAPI.Query<RefRO<utils.SetActiveComponent>>()
            .WithEntityAccess())
        {
            //Debug.Log("Enabling " + activeComp.ValueRO.entity.ToString());

            //if (!state.EntityManager.HasComponent<GameObject>(activeComp.ValueRO.entity))
            //{
            //    Debug.LogWarning($"[SetActiveSystem] Entity {activeComp.ValueRO.entity} has NO GameObject.");
            //}
            //else
            //{
            //    Debug.Log($"[SetActiveSystem] Entity {activeComp.ValueRO.entity} has a GameObject!");
            //}

            //var gameObject = state.EntityManager.GetComponentObject<Transform>(activeComp.ValueRO.entity).gameObject;
            ////gameObject.SetActive(activeComp.ValueRO.isActive);
            //gameObject.SetActive(false);

            //Helper.EnableComponent<utils.SetActiveComponent>(ref state, entity, false);
        }
    }
}
