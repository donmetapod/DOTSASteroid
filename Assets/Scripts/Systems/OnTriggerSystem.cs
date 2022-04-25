using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using Unity.Physics.Systems;

public partial class OnTriggerSystem : SystemBase
{
    private BuildPhysicsWorld buildPhysicsWorld;
    private StepPhysicsWorld stepPhysicsWorld;
    private EntityQuery entityQuery;
    
    protected override void OnCreate()
    {
        buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
        
        
        entityQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[]{typeof(TriggerEventChecker)}   
        });
        RequireForUpdate(entityQuery);
    }
    
    public struct CollectTriggerEventsJob : ITriggerEventsJob
    {
        public NativeList<TriggerEvent> triggerEvents;
        
        public void Execute(TriggerEvent triggerEvent)
        {
            triggerEvents.Add(triggerEvent);
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;
            EntityCommandBuffer entityCommandBuffer = GameReferenceSystem.CommandBufferSystem.CreateCommandBuffer();
            
            //TODO find a cleaner way to check for triggers
            #region Bullets and Asteroids triggers
            // Bullet entity A collides with asteroid Entity B
            if (GameReferenceSystem.allBullets.HasComponent(entityA) && GameReferenceSystem.allAsteroids.HasComponent(entityB))
            {
                KillAsteroidEntity(entityCommandBuffer, entityA, entityB);
            }else if (GameReferenceSystem.allAsteroids.HasComponent(entityA) && GameReferenceSystem.allBullets.HasComponent(entityB)) // Bullet entity B collides with asteroid Entity A
            {
                KillAsteroidEntity(entityCommandBuffer, entityB, entityA);
            }
            #endregion

            #region Player and Asteroids triggers
            if (GameReferenceSystem.AllPlayers.HasComponent(entityA) && GameReferenceSystem.allAsteroids.HasComponent(entityB))
            {
                if (!GameManager.Instance.SpaceshipHasShield)
                {
                    LoseLife(entityCommandBuffer, entityA);
                }
                else
                {
                    entityCommandBuffer.DestroyEntity(entityB);
                    GameManager.Instance.SpaceshipHasShield = false;
                }
            }
            #endregion

            #region Player and PowerUps triggers

            if (GameReferenceSystem.allPowerUps.HasComponent(entityA) && GameReferenceSystem.AllPlayers.HasComponent(entityB))
            {
                GetPowerUp(entityA, entityCommandBuffer);
            }else if (GameReferenceSystem.AllPlayers.HasComponent(entityA) && GameReferenceSystem.allPowerUps.HasComponent(entityB))
            {
                GetPowerUp(entityB, entityCommandBuffer);
            }

            #endregion

            #region Bullets and UFO
            if (GameReferenceSystem.allUFOs.HasComponent(entityA) && GameReferenceSystem.allBullets.HasComponent(entityB))
            {
                KillUFO(entityA, entityCommandBuffer, entityB);
            }else if (GameReferenceSystem.allBullets.HasComponent(entityA) && GameReferenceSystem.allUFOs.HasComponent(entityB))
            {
                KillUFO(entityB, entityCommandBuffer, entityA);
            }
            #endregion

            #region UFO Bullets with spaceship
            if (GameReferenceSystem.AllPlayers.HasComponent(entityA) && GameReferenceSystem.allBullets.HasComponent(entityB))
            {
                LoseLife(entityCommandBuffer, entityA);
            }else if (GameReferenceSystem.allBullets.HasComponent(entityA) && GameReferenceSystem.AllPlayers.HasComponent(entityB))
            {
                LoseLife(entityCommandBuffer, entityB);
            }

            #endregion

            #region UFO and Spaceship
            if (GameReferenceSystem.AllPlayers.HasComponent(entityA) && GameReferenceSystem.allUFOs.HasComponent(entityB))
            {
                LoseLife(entityCommandBuffer, entityA);
            }else if (GameReferenceSystem.allUFOs.HasComponent(entityA) && GameReferenceSystem.AllPlayers.HasComponent(entityB))
            {
                LoseLife(entityCommandBuffer, entityB);
            }
            #endregion
        }

        private static void KillUFO(Entity entityA, EntityCommandBuffer entityCommandBuffer, Entity entityB)
        {
            UFOData defaultUFOData = GameReferenceSystem.allUFOs[entityA];
            UFOData newUFOData = defaultUFOData;
            newUFOData.ResetUFOData = true;
            entityCommandBuffer.SetComponent(entityA, newUFOData);
            Entity vfxFClone = entityCommandBuffer.Instantiate(GameReferenceSystem.allUFOs[entityA].DamageVFX);
            Translation position = new Translation();
            position.Value = GameReferenceSystem.allUFOs[entityA].LastKnownTranslation.Value;
            entityCommandBuffer.SetComponent(vfxFClone, position);
            position.Value = new float3(50, 50, 50);
            entityCommandBuffer.SetComponent(entityB, position); //Moves bullet outside the screen
            GameManager.Instance.Score += 100;
        }

        private static void GetPowerUp(Entity entityA, EntityCommandBuffer entityCommandBuffer)
        {
            // Get power up type
            if (GameReferenceSystem.allPowerUps[entityA].PowerUpType == PowerUpData.PowerUpTypeEnum.Shield)
            {
                GameManager.Instance.SpaceshipHasShield = true;
            }
            else
            {
                GameManager.Instance.SpreadShotIsEnabled = true;
            }

            entityCommandBuffer.DestroyEntity(entityA);
        }

        private static void LoseLife(EntityCommandBuffer entityCommandBuffer, Entity playerEntity)
        {
            if (!GameManager.Instance.PlayerRespawning)
            {
                Entity explosionClone = entityCommandBuffer.Instantiate(GameReferenceSystem.AllPlayers[playerEntity].DestroyVfx);
                Translation position = GameReferenceSystem.AllPlayers[playerEntity].LastKnowPosition;
                entityCommandBuffer.SetComponent(explosionClone, position);
                GameManager.Instance.PlayerRespawning = true;
                GameManager.Instance.PlayerLives--;
            }
        }

        private static void KillAsteroidEntity(EntityCommandBuffer entityCommandBuffer, Entity attackerEntity, Entity damagedEntity)
        {
            BulletData inactiveBullet = new BulletData
            {
                IsActive = false,
                Speed = 0
            };
            entityCommandBuffer.SetComponent(attackerEntity, inactiveBullet);
            Translation defaultBulletTranslation = new Translation();
            defaultBulletTranslation.Value = new float3(100, 100, 100);
            entityCommandBuffer.SetComponent(attackerEntity, defaultBulletTranslation);

            AsteroidData destroyedAsteroidData = GameReferenceSystem.allAsteroids[damagedEntity];
            if (!destroyedAsteroidData.IsSmallAsteroid)
            {
                // Create two small asteroids when a big one is destroyed
                for (int i = 0; i < 2; i++) 
                {
                    Entity smallAsteroidClone = entityCommandBuffer.Instantiate(destroyedAsteroidData.SmallerAsteroid);
                    Translation smallAsteroidTranslation = destroyedAsteroidData.LastKnownTranslation;
                    smallAsteroidTranslation.Value.x += i;
                    smallAsteroidTranslation.Value.y += i;
                    entityCommandBuffer.SetComponent(smallAsteroidClone, smallAsteroidTranslation);
                }
            }

            Entity explosionVFX = entityCommandBuffer.Instantiate(GameReferenceSystem.allAsteroids[damagedEntity].ExplosionVfx);
            Translation position = new Translation
            {
                Value = destroyedAsteroidData.LastKnownTranslation.Value
            };
            entityCommandBuffer.SetComponent(explosionVFX, position);
            entityCommandBuffer.DestroyEntity(damagedEntity);
            GameManager.Instance.Score += 50;
        }
    }

    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        this.RegisterPhysicsRuntimeSystemReadOnly();
    }
    
    protected override void OnUpdate()
    {
        Dependency.Complete();
    
        NativeList<TriggerEvent> triggerEvents = new NativeList<TriggerEvent>(Allocator.TempJob);
    
        var collectTriggerEventsJob = new CollectTriggerEventsJob
        {
            triggerEvents = triggerEvents
        };
    
        var handle = collectTriggerEventsJob.Schedule(stepPhysicsWorld.Simulation, Dependency);
        handle.Complete();
    
        var physicsWorld = buildPhysicsWorld.PhysicsWorld;
        // int expectedNumberOfTriggerEvents = 0;
        
        Entities
            .WithReadOnly(physicsWorld)
            .WithReadOnly(triggerEvents)
            .WithoutBurst()
            .ForEach((ref Entity entity, ref TriggerEventChecker eventChecker) =>
            {
                int numTriggerEvents = 0;
                TriggerEvent triggerEvent = default;

                for (int i = 0; i < triggerEvents.Length; i++)
                {
                    if (triggerEvents[i].EntityA == entity || triggerEvents[i].EntityB == entity)
                    {
                        triggerEvent = triggerEvents[i];
                        numTriggerEvents++;
                    }
                }
                
                if (numTriggerEvents == 0)
                {
                    return;
                }
                // Even if component.NumExpectedEvents is > 1, we still take one trigger event, and not all, because the only
                // difference will be in ColliderKeys which we're not checking here
                int nonTriggerBodyIndex = triggerEvent.EntityA == entity ? triggerEvent.BodyIndexA : triggerEvent.BodyIndexB;
                int triggerBodyIndex = triggerEvent.EntityA == entity ? triggerEvent.BodyIndexB : triggerEvent.BodyIndexA;
                
    
                RigidBody nonTriggerBody = physicsWorld.Bodies[nonTriggerBodyIndex];
                RigidBody triggerBody = physicsWorld.Bodies[triggerBodyIndex];

            }).Run();

        triggerEvents.Dispose();
    }

}
