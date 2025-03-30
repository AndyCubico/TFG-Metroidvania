using Unity.Entities;
using UnityEngine;

public class CombatAuthoring : MonoBehaviour
{
    public class Baker : Baker<CombatAuthoring>
    {
        public override void Bake(CombatAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new CombatComponent
            {
            });
        }
    }
}

public struct CombatComponent : IComponentData
{

}