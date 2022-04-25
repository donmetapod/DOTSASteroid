using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

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
                if (distance > 1000 || bulletData.Collided)// If far from play area or collided, return to pool
                {
                    bulletData.IsActive = false;
                    position.Value = new float3(500, 500, 500); // Default position outside screen
                }
            }
        }).Run();
    }
}
