using TMG.NFE_Tutorial;
using Unity.Entities;
using UnityEngine;

namespace Bakers
{
    public class MainCameraAuthoring : MonoBehaviour
    {
        private class MainCameraAuthoringBaker : Baker<MainCameraAuthoring>
        {
            public override void Bake(MainCameraAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponentObject(entity, new MainCamera());
                AddComponent<MainCameraTag>(entity);
            }
        }
    }
}