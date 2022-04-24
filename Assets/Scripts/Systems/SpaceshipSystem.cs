using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Rendering;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public partial class SpaceshipSystem : SystemBase
{
    private List<Entity> bulletPool = new List<Entity>();
    private float respawnTime = 2;
    
    protected override void OnUpdate()
    {
        
        float deltaTime = Time.DeltaTime;
        Translation shipTranslation = new Translation();
        Rotation shipRotation = new Rotation();
        Entities
            .ForEach((Entity shipEntity, ref Rotation rotation, ref Translation position, ref SpaceshipData ship, 
                ref PhysicsVelocity physicsVelocity, ref PhysicsMass physicsMass,
                in LocalToWorld localToWorld, in InputData inputData) =>
            {
                #region RespawnLogic
                if (GameManager.Instance.PlayerRespawning)
                {
                    respawnTime -= deltaTime;
                    position.Value = new float3(-1000, -1000, -1000);
                }else if (respawnTime <= 0)
                {
                    position.Value = float3.zero;
                    rotation.Value = quaternion.identity;
                    physicsVelocity.Linear = float3.zero;
                    respawnTime = 1;
                }
                #endregion
                
                if(GameManager.Instance.GameState == GameManager.GameStateEnum.GameOver)
                    return;
                
                shipTranslation = position;
                shipRotation = rotation;
                
                bool isRightKeyPressed = Input.GetKey(inputData.rightKey);
                bool isLeftKeyPressed = Input.GetKey(inputData.leftKey);
                bool isUpKeyPressed = Input.GetKey(inputData.upKey);
                bool isDownKeyPressed = Input.GetKey(inputData.downKey);

                ship.Direction.x = Convert.ToInt32(isRightKeyPressed);
                ship.Direction.x -= Convert.ToInt32(isLeftKeyPressed);
                ship.Direction.z = Convert.ToInt32(isUpKeyPressed);
                ship.Direction.z -= Convert.ToInt32(isDownKeyPressed);
                
                rotation.Value = math.mul(math.normalize(rotation.Value), quaternion.AxisAngle(math.forward(), -ship.Direction.x * ship.TurnSpeed * deltaTime));
                
                //Ship physics movement 
                if (isUpKeyPressed)
                {
                    var forceVector = localToWorld.Up * ship.MovementForce * deltaTime;
                    physicsVelocity.ApplyLinearImpulse(physicsMass, forceVector);
                }

                // Enable or disable shield object
                EntityManager.SetEnabled(ship.Shield, GameManager.Instance.SpaceshipHasShield);
                
                // Enable or disable spread shot object
                EntityManager.SetEnabled(ship.SpreadShot, GameManager.Instance.SpreadShotIsEnabled);
                
            }).WithStructuralChanges().WithoutBurst().Run();
        
        
        Entities.ForEach((Entity entity,ref BulletData bulletData) =>
        {
            if (!bulletData.AddedToPool && !bulletData.UFOBullet)
            {
                bulletPool.Add(entity);
                bulletData.AddedToPool = true;
            }
        }).WithoutBurst().Run();
        

        // Shoot bullet from pool
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("shoot bullet");
            GameManager.Instance.PlayAudioClipWithName("Shoot");
            EndSimulationEntityCommandBufferSystem commandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            EntityCommandBuffer entityCommandBuffer = commandBufferSystem.CreateCommandBuffer();
            ComponentDataFromEntity<BulletData> allBulletData = GetComponentDataFromEntity<BulletData>(true);
            
            BulletData activeBullet = new BulletData
            {
                IsActive = true,
                Speed = 100
            };

            int spreadShotCount = 0;
            int[] spreadShotRotations = {-8, 8, 8 };// Rotations for each one of the spread shot bullets
            for (int i = 0; i < bulletPool.Count; i++)
            {
                if (GameManager.Instance.SpreadShotIsEnabled)
                {
                    BulletData currentBulletData = allBulletData[bulletPool[i]];
                    if (!currentBulletData.IsActive)
                    {
                        quaternion zRot = quaternion.RotateZ(spreadShotRotations[spreadShotCount] * Mathf.Deg2Rad);
                        shipRotation.Value = math.mul(shipRotation.Value, zRot);
                        entityCommandBuffer.SetComponent(bulletPool[i], activeBullet);
                        entityCommandBuffer.SetComponent(bulletPool[i], shipTranslation);
                        entityCommandBuffer.SetComponent(bulletPool[i], shipRotation);
                        spreadShotCount++;
                        if(spreadShotCount >= 3)
                            break;
                    }
                }
                else
                {
                    BulletData currentBulletData = allBulletData[bulletPool[i]];
                    if (!currentBulletData.IsActive)
                    {
                        entityCommandBuffer.SetComponent(bulletPool[i], activeBullet);
                        entityCommandBuffer.SetComponent(bulletPool[i], shipTranslation);
                        entityCommandBuffer.SetComponent(bulletPool[i], shipRotation);
                        break;
                    } 
                }
                
            }
        }
    }

}

