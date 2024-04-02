using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;

namespace Common
{
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial struct NpcAttackSystem : ISystem
    {
        
    }
    
    [BurstCompile]
    [WithAll(typeof(Simulate))]
    public partial struct NpcAttackJob : IJobEntity
    {
        [ReadOnly] public NetworkTick CurrentTick;
        [ReadOnly] public ComponentLookup<LocalTransform> TransformLookup;
        
        public EntityCommandBuffer.ParallelWriter ECB;

        [BurstCompile]
        private void Execute(ref DynamicBuffer<NpcAttackCooldown> npcAttackCooldown, in NpcAttackProperty attackProperty, 
            in NpcTargetEntity targetEntity, Entity npcEntity, MobaTeam team, [ChunkIndexInQuery] int sortKey)
        {
            if (!TransformLookup.HasComponent(targetEntity.Value)) return;
            if (!npcAttackCooldown.GetDataAtTick(CurrentTick, out var cooldownExpirationTick))
            {
                cooldownExpirationTick.Value = NetworkTick.Invalid;
            }
            
            var canAttack = !cooldownExpirationTick.Value.IsValid || CurrentTick.IsNewerThan(cooldownExpirationTick.Value);
            
            if (!canAttack) return;
            
            
        }
    }
}