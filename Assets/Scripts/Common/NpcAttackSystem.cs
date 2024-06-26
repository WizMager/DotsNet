﻿using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

namespace Common
{
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial struct NpcAttackSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NetworkTime>();
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var networkTime = SystemAPI.GetSingleton<NetworkTime>();

            state.Dependency = new NpcAttackJob
            {
                CurrentTick = networkTime.ServerTick,
                TransformLookup = SystemAPI.GetComponentLookup<LocalTransform>(),
                ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
            }.ScheduleParallel(state.Dependency);
        }
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

            var spawnPosition = TransformLookup[npcEntity].Position + attackProperty.FirePointOffset;
            var targetPosition = TransformLookup[targetEntity.Value].Position;

            var newAttack = ECB.Instantiate(sortKey, attackProperty.AttackPrefab);
            var newAttackTransform = LocalTransform.FromPositionRotation(spawnPosition,
                quaternion.LookRotationSafe(targetPosition - spawnPosition, math.up()));
            
            ECB.SetComponent(sortKey, newAttack, newAttackTransform);
            ECB.SetComponent(sortKey, newAttack, team);

            var newCooldownTick = CurrentTick;
            newCooldownTick.Add(attackProperty.CooldownTickCount);
            npcAttackCooldown.AddCommandData(new NpcAttackCooldown
            {
                Tick = CurrentTick, Value = newCooldownTick
            });
        }
    }
}