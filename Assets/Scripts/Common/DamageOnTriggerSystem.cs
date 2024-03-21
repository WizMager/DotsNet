using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;

namespace Common
{
    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [UpdateAfter(typeof(PhysicsSimulationGroup))]
    public partial struct DamageOnTriggerSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SimulationSingleton>();
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            
            var damageOnTriggerJob = new DamageOnTriggerJob
            {
                DamageOnTriggerLookup = SystemAPI.GetComponentLookup<DamageOnTrigger>(true),
                TeamLookup = SystemAPI.GetComponentLookup<MobaTeam>(true),
                AlreadyDamagedLookup = SystemAPI.GetBufferLookup<AlreadyDamagedEntity>(true),
                DamageBufferLookup = SystemAPI.GetBufferLookup<DamageBufferElement>(true),
                Ecb = ecb
            };

            var simulationSingleton = SystemAPI.GetSingleton<SimulationSingleton>();
            state.Dependency = damageOnTriggerJob.Schedule(simulationSingleton, state.Dependency);
        }
    }

    public struct DamageOnTriggerJob : ITriggerEventsJob
    {
        [ReadOnly] public ComponentLookup<DamageOnTrigger> DamageOnTriggerLookup;
        [ReadOnly] public ComponentLookup<MobaTeam> TeamLookup;
        [ReadOnly] public BufferLookup<AlreadyDamagedEntity> AlreadyDamagedLookup;
        [ReadOnly] public BufferLookup<DamageBufferElement> DamageBufferLookup;

        public EntityCommandBuffer Ecb;
        
        public void Execute(TriggerEvent triggerEvent)
        {
            Entity damageDealerEntity;
            Entity damageReceiverEntity;

            if (DamageBufferLookup.HasBuffer(triggerEvent.EntityA) && DamageOnTriggerLookup.HasComponent(triggerEvent.EntityB))
            {
                damageDealerEntity = triggerEvent.EntityB;
                damageReceiverEntity = triggerEvent.EntityA;
            }else if (DamageBufferLookup.HasBuffer(triggerEvent.EntityB) && DamageOnTriggerLookup.HasComponent(triggerEvent.EntityA))
            {
                damageDealerEntity = triggerEvent.EntityA;
                damageReceiverEntity = triggerEvent.EntityB;
            }
            else
            {
                return;
            }

            var alreadyDamagedBuffer = AlreadyDamagedLookup[damageDealerEntity];
            foreach (var alreadyDamagedEntity in alreadyDamagedBuffer)
            {
                if (alreadyDamagedEntity.Value.Equals(damageReceiverEntity)) return;
            }

            if (TeamLookup.TryGetComponent(damageDealerEntity, out var damageDealingTeam) 
                && TeamLookup.TryGetComponent(damageReceiverEntity, out var damageReceivingTeam))
            {
                if (damageDealingTeam.Value == damageReceivingTeam.Value) return;
            }

            var damageOnTrigger = DamageOnTriggerLookup[damageDealerEntity];
            Ecb.AppendToBuffer(damageReceiverEntity, new DamageBufferElement{ Value = damageOnTrigger.Value});
            Ecb.AppendToBuffer(damageDealerEntity, new AlreadyDamagedEntity{ Value = damageReceiverEntity});
        }
    }
}