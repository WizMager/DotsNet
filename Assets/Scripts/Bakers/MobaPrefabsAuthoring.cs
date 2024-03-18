using Common;
using Unity.Entities;
using UnityEngine;

namespace Bakers
{
    public class MobaPrefabsAuthoring : MonoBehaviour
    {
        public GameObject Champion;
        
        private class MobaPrefabsAuthoringBaker : Baker<MobaPrefabsAuthoring>
        {
            public override void Bake(MobaPrefabsAuthoring authoring)
            {
                var prefabContainerEntity = GetEntity(TransformUsageFlags.None);
                AddComponent(prefabContainerEntity, new MobaPrefabs
                {
                    Champion = GetEntity(authoring.Champion, TransformUsageFlags.Dynamic)
                });
            }
        }
    }
}