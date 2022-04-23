using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public partial class UFOSystem : SystemBase
{
    
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        Entities.ForEach((Entity entity, int entityInQueryIndex,ref Translation position, ref Rotation rotation, ref UFOData ufoData) =>
        {
            ufoData.GameTime += deltaTime;
            ufoData.RandomValue = Random.CreateFromIndex((uint)ufoData.GameTime);
            if (!ufoData.InAction)
            {
                ufoData.StartActionAccumulatedTime += deltaTime;
                if (ufoData.StartActionAccumulatedTime > ufoData.StartActionDelayTime)
                {
                    ufoData.InAction = true;
                    // Set initial direction
                    ufoData.MoveDirection.x = position.Value.x > 0 ? -ufoData.MovementSpeed : ufoData.MovementSpeed;
                    
                    //Set initial random Y position
                    position.Value.y = ufoData.RandomValue.NextFloat(-ufoData.YScreenLimit, ufoData.YScreenLimit);
                }
                return;
            }
            
            // Move UFO
            position.Value += ufoData.MoveDirection * deltaTime;
            
            //Change direction after some time
            ufoData.DirectionChangeAccumulatedTime += deltaTime;
            if (ufoData.DirectionChangeAccumulatedTime > ufoData.DirectionChangeTime && !ufoData.alreadyChangedDirecion)
            {
                ufoData.alreadyChangedDirecion = true;
                int yMultiplier = 1;
                float rnd = ufoData.RandomValue.NextInt(0, 100); // 50% chance of changing y movement downwards
                if (rnd < 50)
                {
                    yMultiplier = -1;
                }
                ufoData.MoveDirection.y = ufoData.MovementSpeed * yMultiplier;
            }
            
            // If too far from center screen, move back to initial position
            float distanceFromCenter = math.distancesq(position.Value, float3.zero);
            if (distanceFromCenter > ufoData.DistanceFromCenterResetValue)
            {
                // Return to one of random initial positions
                float rnd = ufoData.RandomValue.NextInt(0, 100);
                position.Value = rnd < 50 ? new float3(50, 0, 0) : new float3(-50, 0, 0);
                
                // Reset UFO values
                ufoData.InAction = false;
                ufoData.MoveDirection = float3.zero;
                ufoData.StartActionAccumulatedTime = 0;
                ufoData.DirectionChangeAccumulatedTime = 0;
                ufoData.alreadyChangedDirecion = false;
                
                // Create new values for next attack
                ufoData.StartActionDelayTime +=
                    ufoData.RandomValue.NextInt(-ufoData.StartActionOffsetValue, ufoData.StartActionOffsetValue);
                ufoData.DirectionChangeTime += 
                    ufoData.RandomValue.NextInt(-ufoData.DirectionChangeOffsetValue, ufoData.DirectionChangeOffsetValue);
            }

            // Debug.Log("distance from screen center " + distanceFromCenter);
        }).Schedule();
    }
}
