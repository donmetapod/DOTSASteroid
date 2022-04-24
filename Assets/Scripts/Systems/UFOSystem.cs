using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public partial class UFOSystem : SystemBase
{
    List<Entity> bulletPoolUFO = new List<Entity>();
    
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity entity,ref BulletData bulletData) =>
        {
            if (!bulletData.AddedToPool && bulletData.UFOBullet)
            {
                bulletPoolUFO.Add(entity);
                bulletData.AddedToPool = true;
            }
        }).WithoutBurst().Run();
        
        float deltaTime = Time.DeltaTime;

        Entities.ForEach((ref Translation position, ref Rotation rotation, ref UFOData ufoData) =>
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
                    
                    // Start accumulating time until direction change
                    ufoData.DirectionChangeAccumulatedTime = 0;
                    
                    //Used for UFO sfx
                    GameManager.Instance.UfoInAction = true;
                }
                return;
            }
            
            // Move UFO
            position.Value += ufoData.MoveDirection * deltaTime;
            ufoData.LastKnownTranslation = position;
            
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
            
            // If too far from center screen, reset UFO
            float distanceFromCenter = math.distancesq(position.Value, float3.zero);
            if (distanceFromCenter > ufoData.DistanceFromCenterResetValue)
            {
                ufoData.ResetUFOData = true;
            }

            if (ufoData.ResetUFOData)
            {
                ufoData.ResetUFOData = false;
                // Return to one of random initial positions
                float randomSide = ufoData.RandomValue.NextInt(0, 100);
                float randomPositionOffset = ufoData.RandomValue.NextInt(30, 80);
                position.Value = randomSide < 50 ? new float3(randomPositionOffset, 0, 0) : new float3(-randomPositionOffset, 0, 0);
                
                // Reset UFO values
                ufoData.InAction = false;
                ufoData.MoveDirection = float3.zero;
                ufoData.StartActionAccumulatedTime = 0;
                ufoData.DirectionChangeAccumulatedTime = 0;
                ufoData.alreadyChangedDirecion = false;
                //Used for UFO sfx
                GameManager.Instance.UfoInAction = false;
            }

            ufoData.ShootAccumulationTime += deltaTime;
            
            // Shoot bullet from UFO bullets pool
            if (ufoData.ShootAccumulationTime > ufoData.FireRate)
            {
                ufoData.ShootAccumulationTime = 0;
                EndSimulationEntityCommandBufferSystem commandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
                EntityCommandBuffer entityCommandBuffer = commandBufferSystem.CreateCommandBuffer();
                ComponentDataFromEntity<BulletData> allBulletData = GetComponentDataFromEntity<BulletData>(true);
                GameManager.Instance.PlayAudioClipWithName("Shoot");
                BulletData activeBullet = new BulletData
                {
                    IsActive = true,
                    Speed = 100
                };
                
                for (int i = 0; i < bulletPoolUFO.Count; i++)
                {
                    BulletData currentBulletData = allBulletData[bulletPoolUFO[i]];
                    if (!currentBulletData.IsActive)
                    {
                        entityCommandBuffer.SetComponent(bulletPoolUFO[i], activeBullet);
                        entityCommandBuffer.SetComponent(bulletPoolUFO[i], position);
                        // Random bullet direction
                        Rotation bulletRotation = rotation;
                        float rnd = ufoData.RandomValue.NextInt(0, 360);
                        quaternion zRot = quaternion.RotateZ(rnd * Mathf.Deg2Rad);
                        bulletRotation.Value = math.mul(bulletRotation.Value, zRot);
                        entityCommandBuffer.SetComponent(bulletPoolUFO[i], bulletRotation);
                        break;
                    } 
                }
            }

            
        }).WithoutBurst().Run();
    }
}
