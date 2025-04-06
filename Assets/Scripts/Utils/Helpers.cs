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

    // Prefer this over EnableComponent
    public static void EnableComponent<T>(in Entity entity, bool enable = true) where T : unmanaged, QG_IEnableComponent
    {
        EntityManager entityManager = GetEntityManager();
        entityManager.SetComponentEnabled<T>(entity, enable);
    }

    public static void EnableComponentFromState(ref SystemState state, in Entity entity, ComponentType type, bool enable = true)
    {
        state.EntityManager.SetComponentEnabled(entity, type, enable);
    }

}