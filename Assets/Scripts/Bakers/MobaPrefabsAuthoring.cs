using Common;
using Unity.Entities;
using UnityEngine;

namespace Bakers
{
    public class MobaPrefabsAuthoring : MonoBehaviour
    {
        [Header("Entities: ")]
        public GameObject Champion;

        [Space] 
        [Header("GameObjects: ")] 
        public GameObject HealthBarPrefab;
        
        private class MobaPrefabsAuthoringBaker : Baker<MobaPrefabsAuthoring>
        {
            public override void Bake(MobaPrefabsAuthoring authoring)
            {
                var prefabContainerEntity = GetEntity(TransformUsageFlags.None);
                AddComponent(prefabContainerEntity, new MobaPrefabs
                {
                    Champion = GetEntity(authoring.Champion, TransformUsageFlags.Dynamic)
                });
                
                AddComponentObject(prefabContainerEntity, new UIPrefabs{ HealthBar = authoring.HealthBarPrefab });
            }
        }
    }
}