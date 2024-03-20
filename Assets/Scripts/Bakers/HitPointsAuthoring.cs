using Common;
using Unity.Entities;
using UnityEngine;

namespace Bakers
{
    public class HitPointsAuthoring : MonoBehaviour
    {
        public int MaxHitPoints;
        
        private class HitPointsAuthoringBaker : Baker<HitPointsAuthoring>
        {
            public override void Bake(HitPointsAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new MaxHitPoints{ Value = authoring.MaxHitPoints });
                AddComponent(entity, new CurrentHitPoints{ Value = authoring.MaxHitPoints });
                AddBuffer<DamageBufferElement>(entity);
                AddBuffer<DamageThisTick>(entity);
            }
        }
    }
}