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
public partial class ShipSystem : SystemBase
{
    private List<Entity> bulletPool = new List<Entity>();
   
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
                // ship.Shield.SetActive(false);
                if (Input.GetKey(KeyCode.Space)) // TODO do this with power up
                {
                    GameStateData.Instance.SpaceshipHasShield = true;
                }

                // Enable or disable shield
                EntityManager.SetEnabled(ship.Shield, GameStateData.Instance.SpaceshipHasShield);
                
            }).WithStructuralChanges().WithoutBurst().Run();
        
        
        Entities.ForEach((Entity entity,ref BulletData bulletData) =>
        {
            if (!bulletData.AddedToPool)
            {
                bulletPool.Add(entity);
                bulletData.AddedToPool = true;
            }
        }).WithoutBurst().Run();
        
        
        // Shoot bullet from pool
        if (Input.GetMouseButtonDown(0))
        {
            EndSimulationEntityCommandBufferSystem commandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            EntityCommandBuffer entityCommandBuffer = commandBufferSystem.CreateCommandBuffer();
            ComponentDataFromEntity<BulletData> allBulletData = GetComponentDataFromEntity<BulletData>(true);
            
            BulletData activeBullet = new BulletData
            {
                IsActive = true,
                Speed = 100
            };
            
            for (int i = 0; i < bulletPool.Count; i++)
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

