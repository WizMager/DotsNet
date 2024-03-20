﻿using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

namespace Common
{
    public struct ChampTag : IComponentData {}
    public struct NewChampTag : IComponentData {}
    public struct OwnerChampTag : IComponentData {}
    public struct MobaTeam : IComponentData
    {
        [GhostField] public TeamType Value;
    }

    public struct CharacterMoveSpeed : IComponentData
    {
        public float Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.AllPredicted)]
    public struct ChampMoveTargetPosition : IInputComponentData
    {
        [GhostField(Quantization = 0)] public float3 Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.AllPredicted)]
    public struct AbilityInput : IInputComponentData
    {
        [GhostField] public InputEvent AoeAbility;
    }
}