using Unity.Entities;

[UpdateInGroup(typeof(InitializationSystemGroup))]
partial struct WeatherInitializationSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        foreach (var timerComp
            in SystemAPI.Query<
                RefRW<utils.TimerComponent>>())
        {
            timerComp.ValueRW.timer -= SystemAPI.Time.DeltaTime;

            if (timerComp.ValueRO.timer <= 0)
            {
                if (timerComp.ValueRO.entityHolder != null)
                {
                    if (state.EntityManager.HasComponent(timerComp.ValueRO.entityHolder, timerComp.ValueRO.componentType))
                    {
                        Helper.EnableComponent(timerComp.ValueRO.entityHolder, timerComp.ValueRO.componentType);
                    }
                }
            }
        }
    }
}