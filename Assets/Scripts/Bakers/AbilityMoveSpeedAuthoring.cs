using Common;
using Unity.Entities;
using UnityEngine;

namespace Bakers
{
    public class AbilityMoveSpeedAuthoring : MonoBehaviour
    {
        public float MoveSpeed;
        
        private class AbilityMoveSpeedAuthoringBaker : Baker<AbilityMoveSpeedAuthoring>
        {
            public override void Bake(AbilityMoveSpeedAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new AbilityMoveSpeed{ Value = authoring.MoveSpeed});
            }
        }
    }
}