using Common;
using Unity.Entities;
using Unity.NetCode;

namespace TMG.NFE_Tutorial
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial struct AbilityCooldownUiSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NetworkTime>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var currentTick = SystemAPI.GetSingleton<NetworkTime>().ServerTick;
            var abilityUiCooldownController = AbilityCooldownUIController.Instance;

            foreach (var (cooldownTargetTicks, abilityCooldownTicks) in SystemAPI.Query<DynamicBuffer<AbilityCooldownTargetTicks>, AbilityCooldownTicks>())
            {
                if (!cooldownTargetTicks.GetDataAtTick(currentTick, out var curTargetTicks))
                {
                    curTargetTicks.AoeAbility = NetworkTick.Invalid;
                    curTargetTicks.SkillShotAbility = NetworkTick.Invalid;
                }

                if (curTargetTicks.AoeAbility == NetworkTick.Invalid || currentTick.IsNewerThan(curTargetTicks.AoeAbility))
                {
                    abilityUiCooldownController.UpdateAoeMask(0f);
                }
                else
                {
                    var aoeRemainingTickCount = curTargetTicks.AoeAbility.TickIndexForValidTick - currentTick.TickIndexForValidTick;
                    var fillAmount = (float) aoeRemainingTickCount / abilityCooldownTicks.AoeAbility;
                    abilityUiCooldownController.UpdateAoeMask(fillAmount);
                }
                
                if (curTargetTicks.SkillShotAbility == NetworkTick.Invalid || currentTick.IsNewerThan(curTargetTicks.SkillShotAbility))
                {
                    abilityUiCooldownController.UpdateSkillShotMask(0f);
                }
                else
                {
                    var skillShotRemainingTickCount = curTargetTicks.SkillShotAbility.TickIndexForValidTick - currentTick.TickIndexForValidTick;
                    var fillAmount = (float) skillShotRemainingTickCount / abilityCooldownTicks.SkillShotAbility;
                    abilityUiCooldownController.UpdateSkillShotMask(fillAmount);
                }
            }
        }
    }
}