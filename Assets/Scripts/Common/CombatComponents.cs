﻿using Unity.Entities;
using Unity.NetCode;

namespace Common
{
    public struct MaxHitPoints : IComponentData
    {
        public int Value;
    }
    
    public struct CurrentHitPoints : IComponentData
    {
        [GhostField] public int Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.All)]
    public struct DamageBufferElement : IBufferElementData
    {
        public int Value;
    }

    [GhostComponent(PrefabType = GhostPrefabType.All, OwnerSendType = SendToOwnerType.SendToNonOwner)]
    public struct DamageThisTick : ICommandData
    {
        public NetworkTick Tick { get; set; }
        public int Value;
    }

    public struct AbilityPrefabs : IComponentData
    {
        public Entity AoeAbility;
    }

    public struct DestroyOnTimer : IComponentData
    {
        public float Value;
    }

    public struct DestroyAtTick : IComponentData
    {
        [GhostField]public NetworkTick Value;
    }
    
    public struct DestroyEntityTag : IComponentData {}
}