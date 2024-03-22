using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace Common
{
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial struct BeginSkillShotSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NetworkTime>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            var netTime = SystemAPI.GetSingleton<NetworkTime>();
            
            if (!netTime.IsFirstTimeFullyPredictingTick) return;

            var currentTick = netTime.ServerTick;
            var isServer = state.WorldUnmanaged.IsServer();

            foreach (var skillShot in SystemAPI.Query<SkillShotAspect>().WithAll<Simulate>().WithNone<AimSkillShotTag>())
            {
                var isOnCooldown = true;

                for (var i = 0u; i < netTime.SimulationStepBatchSize; i++)
                {
                    var testTick = currentTick;
                    testTick.Subtract(i);

                    if (!skillShot.CooldownTargetTicks.GetDataAtTick(currentTick, out var curTargetTick))
                    {
                        curTargetTick.SkillShotAbility = NetworkTick.Invalid;
                    }

                    if (curTargetTick.SkillShotAbility == NetworkTick.Invalid ||
                        !curTargetTick.SkillShotAbility.IsNewerThan(currentTick))
                    {
                        isOnCooldown = false;
                        break;
                    }
                }

                if (isOnCooldown) return;

                if (!skillShot.BeginAttack) continue;
                
                ecb.AddComponent<AimSkillShotTag>(skillShot.ChampionEntity);
            }

            foreach (var skillShot in SystemAPI.Query<SkillShotAspect>().WithAll<AimSkillShotTag, Simulate>())
            {
                if (!skillShot.ConfirmAttack) continue;
                var skillShotAbility = ecb.Instantiate(skillShot.AbilityPrefab);

                var newPosition = skillShot.SpawnPosition;
                ecb.SetComponent(skillShotAbility, newPosition);
                ecb.SetComponent(skillShotAbility, skillShot.Team);
                ecb.RemoveComponent<AimSkillShotTag>(skillShot.ChampionEntity);

                if (isServer) continue;
                skillShot.CooldownTargetTicks.GetDataAtTick(currentTick, out var curTargetTick);

                var newCooldownTargetTick = currentTick;
                newCooldownTargetTick.Add(skillShot.CooldownTicks);
                curTargetTick.SkillShotAbility = newCooldownTargetTick;

                var nextTick = currentTick;
                nextTick.Add(1u);
                curTargetTick.Tick = nextTick;
                
                skillShot.CooldownTargetTicks.AddCommandData(curTargetTick);
            }
            
            ecb.Playback(state.EntityManager);
        }
    }
}