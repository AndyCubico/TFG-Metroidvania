using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;

public static class Helper
{
    public static EntityManager GetEntityManager()
    {
        return World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    public static Entity GetROSingletonComponent<TComponent>(ref SystemState state)
    {
        var entityQuery = state.GetEntityQuery(ComponentType.ReadOnly<TComponent>());
        return entityQuery.GetSingletonEntity(); // exception if there is more than one
    }

    public static Entity GetRWSingletonComponent<TComponent>(ref SystemState state)
    {
        var entityQuery = state.GetEntityQuery(ComponentType.ReadWrite<TComponent>());
        return entityQuery.GetSingletonEntity(); // exception if there is more than one
    }

    public static void EnableComponent<T>(ref SystemState state, in Entity entity, bool enable = true) where T : unmanaged, QG_IEnableComponent
    {
        state.EntityManager.SetComponentEnabled<T>(entity, enable);
    }

    public static void AddComponentWithDisabled<T>(IBaker baker, Entity entity, T component, bool isEnabled = false) where T : unmanaged, QG_IEnableComponent
    {
        baker.AddComponent(entity, component);
        baker.SetComponentEnabled<T>(entity, isEnabled);
    }

    // Class to compare Int2, useful for pathfinding, maybe it can have other uses.
    public class Int2Comparer : IEqualityComparer<int2>
    {
        public bool Equals(int2 a, int2 b)
        {
            return a.x == b.x && a.y == b.y;
        }

        public int GetHashCode(int2 obj)
        {
            return obj.x.GetHashCode() ^ obj.y.GetHashCode();
        }
    }
}