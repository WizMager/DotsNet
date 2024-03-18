using Unity.NetCode;

namespace Common
{
    public struct MobaTeamRequest : IRpcCommand
    {
        public TeamType Value;
    }
}