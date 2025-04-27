using Unity.Entities;
using UnityEngine;

public class SetActiveAuthoring : MonoBehaviour
{
    public class Baker : Baker<SetActiveAuthoring>
    {
        public override void Bake(SetActiveAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.WorldSpace);
            AddComponent(entity, new utils.SetActiveComponent { });
            SetComponentEnabled<utils.SetActiveComponent>(entity, false);

            AddComponentObject(entity, authoring.gameObject);
        }
    }
}
