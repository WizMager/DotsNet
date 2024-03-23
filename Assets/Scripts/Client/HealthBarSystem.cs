using Common;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

namespace TMG.NFE_Tutorial
{
    [UpdateAfter(typeof(TransformSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial struct HealthBarSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<UIPrefabs>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (transform, healthBarOffset, maxHitPoints, entity) in SystemAPI.Query<LocalTransform, HealthBarOffset, MaxHitPoints>().WithAll<HealthBarUiReference>().WithEntityAccess())
            {
                var healthBarPrefab = SystemAPI.ManagedAPI.GetSingleton<UIPrefabs>().HealthBar;
                var spawnPosition = transform.Position + healthBarOffset.Value;
                var newHealthBar = Object.Instantiate(healthBarPrefab, spawnPosition, Quaternion.identity);
                SetHealthBar(newHealthBar, maxHitPoints.Value, maxHitPoints.Value);
                ecb.AddComponent(entity, new HealthBarUiReference{ Value = newHealthBar });
            }
        }

        private void SetHealthBar(GameObject healthBarCanvasObject, int curHitPoints, int maxHitPoints)
        {
            var healthBarSlider = healthBarCanvasObject.GetComponentInChildren<Slider>();
            healthBarSlider.minValue = 0;
            healthBarSlider.maxValue = maxHitPoints;
            healthBarSlider.value = curHitPoints;
        }
    }
}