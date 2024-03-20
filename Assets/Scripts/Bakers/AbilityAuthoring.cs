using Common;
using Unity.Entities;
using UnityEngine;

namespace Bakers
{
    public class AbilityAuthoring : MonoBehaviour
    {
        public GameObject AoeAbility;
        
        private class AbilityAuthoringBaker : Baker<AbilityAuthoring>
        {
            public override void Bake(AbilityAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new AbilityPrefabs
                {
                    AoeAbility = GetEntity(authoring.AoeAbility, TransformUsageFlags.Dynamic)
                });
            }
        }
    }
}