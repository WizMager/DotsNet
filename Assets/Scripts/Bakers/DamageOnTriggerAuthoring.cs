using Common;
using Unity.Entities;
using UnityEngine;

namespace Bakers
{
    public class DamageOnTriggerAuthoring : MonoBehaviour
    {
        public int DamageOnTrigger;
        
        private class DamageOnTriggerAuthoringBaker : Baker<DamageOnTriggerAuthoring>
        {
            public override void Bake(DamageOnTriggerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new DamageOnTrigger{ Value = authoring.DamageOnTrigger });
                AddBuffer<AlreadyDamagedEntity>(entity);
            }
        }
    }
}