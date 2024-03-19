using Unity.Entities;
using Unity.NetCode;

namespace Common
{
    public struct ChampTag : IComponentData {}
    public struct NewChampTag : IComponentData {}

    public struct MobaTeam : IComponentData
    {
        [GhostField] public TeamType Value;
    }
}