using Unity.Entities;

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
}