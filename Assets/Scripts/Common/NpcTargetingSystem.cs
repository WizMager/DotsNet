using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace Common
{
    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [UpdateAfter(typeof(PhysicsSimulationGroup))]
    [UpdateBefore(typeof(ExportPhysicsWorld))]
    public partial struct NpcTargetingSystem : ISystem
    {
        private CollisionFilter _npcAttackFilter;

        public void OnCreate(ref SystemState state)
        {
            _npcAttackFilter = new CollisionFilter
            {
                BelongsTo = 1 << 6,
                CollidesWith = 1 << 1 | 1 << 2 | 1 << 4
            };
        }

        [BurstCompile]
        [WithAll(typeof(Simulate))]
        public partial struct NpcTargetingJob : IJobEntity
        {
            [ReadOnly] public CollisionWorld CollisionWorld;
            [ReadOnly] public CollisionFilter CollisionFilter;
            [ReadOnly] public ComponentLookup<MobaTeam> MobaTeamLookup;

            [BurstCompile]
            private void Execute(Entity npcEntity, ref NpcTargetEntity targetEntity, in LocalTransform transform, in NpcTargetRadius targetRadius)
            {
                var hits = new NativeList<DistanceHit>(Allocator.TempJob);
                
                if (CollisionWorld.OverlapSphere(transform.Position, targetRadius.Value, ref hits, CollisionFilter))
                {
                    
                }
            }
        }
    }
}