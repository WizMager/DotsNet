using Common;
using Unity.Entities;
using UnityEngine;

namespace Bakers
{
    public class DestroyOnTimerAuthoring : MonoBehaviour
    {
        public float DestroyOnTimer;
        
        private class DestroyAtTimerAuthoringBaker : Baker<DestroyOnTimerAuthoring>
        {
            public override void Bake(DestroyOnTimerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new DestroyOnTimer{ Value = authoring.DestroyOnTimer });
            }
        }
    }
}