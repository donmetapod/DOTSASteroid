using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class BulletSystem : SystemBase
{

    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        
        Entities.ForEach((ref BulletData bulletData, ref Translation position, in LocalToWorld localToWorld) =>
        {
            if (bulletData.IsActive)
            {
                position.Value += localToWorld.Up * deltaTime * bulletData.Speed;
                float distance = math.distancesq(float3.zero, position.Value);
                if (distance > 1000)// If far from play area, return to pool
                {
                    bulletData.IsActive = false;
                    position.Value = new float3(100, 100, 100); // Default position outside screen
                }
            }
        }).Run();
    }
}
