using Common;
using Unity.Entities;
using UnityEngine;

namespace Bakers
{
    public class MobaTeamAuthoring : MonoBehaviour
    {
        public TeamType MobaTeam;
        private class MobaTeamAuthoringBaker : Baker<MobaTeamAuthoring>
        {
            public override void Bake(MobaTeamAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new MobaTeam { Value = authoring.MobaTeam });
            }
        }
    }
}