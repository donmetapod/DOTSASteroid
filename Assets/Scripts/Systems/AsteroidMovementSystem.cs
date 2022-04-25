using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public partial class AsteroidMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        Translation shipCurrentPosition = new Translation();
        
        Entities.
            ForEach((in Translation position, in SpaceshipData ship) =>
            {
                shipCurrentPosition = position;
            }).Run();
        
        Entities.ForEach((ref Translation translation, ref Rotation rotation, ref AsteroidData asteroidData) =>
        {
            // Sets MoveDirection to all asteroids
            if (asteroidData.MoveDirection.Equals(float3.zero))
            {
                float3 randomTargetOffset = new float3(Random.Range(-asteroidData.TargetOffset, asteroidData.TargetOffset),
                    Random.Range(-asteroidData.TargetOffset, asteroidData.TargetOffset), 0);
                shipCurrentPosition.Value += randomTargetOffset;
                asteroidData.MoveDirection = shipCurrentPosition.Value - translation.Value;
            }
            
            // Move the asteroids
            translation.Value += asteroidData.MoveDirection * deltaTime * asteroidData.MovementSpeed;
            asteroidData.LastKnownTranslation = translation;
        }).Run();
    }
}
