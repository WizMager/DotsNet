using Common;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace Bakers
{
    public class NpcAttackAuthoring : MonoBehaviour
    {
        public float NpcTargetRadius;
        public Vector3 FirepointOffset;
        public float AttackCooldownTime;
        public GameObject AttackPrefab;

        public NetCodeConfig NetCodeConfig;
        public int SimulationTickRate => NetCodeConfig.ClientServerTickRate.SimulationTickRate;
        
        private class NpcAttackAuthoringBaker : Baker<NpcAttackAuthoring>
        {
            public override void Bake(NpcAttackAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new NpcTargetRadius{ Value = authoring.NpcTargetRadius });
                AddComponent(entity, new NpcAttackProperty
                {
                    FirePointOffset = authoring.FirepointOffset,
                    CooldownTickCount = (uint)(authoring.AttackCooldownTime * authoring.SimulationTickRate),
                    AttackPrefab = GetEntity(authoring.AttackPrefab, TransformUsageFlags.Dynamic)
                });
                AddComponent<NpcTargetEntity>(entity);
                AddComponent<NpcAttackCooldown>(entity);
            }
        }
    }
}