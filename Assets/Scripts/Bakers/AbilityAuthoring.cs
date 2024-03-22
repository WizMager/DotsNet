using Common;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace Bakers
{
    public class AbilityAuthoring : MonoBehaviour
    {
        public GameObject AoeAbility;
        public float AbilityCooldown;
        public NetCodeConfig NetCodeConfig;
        private int SimulationTickRate => NetCodeConfig.ClientServerTickRate.SimulationTickRate;
        
        private class AbilityAuthoringBaker : Baker<AbilityAuthoring>
        {
            public override void Bake(AbilityAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new AbilityPrefabs
                {
                    AoeAbility = GetEntity(authoring.AoeAbility, TransformUsageFlags.Dynamic)
                });
                AddComponent(entity, new AbilityCooldownTicks
                {
                    AoeAbility = (uint)(authoring.AbilityCooldown * authoring.SimulationTickRate)
                });
                AddBuffer<AbilityCooldownTargetTicks>(entity);
            }
        }
    }
}