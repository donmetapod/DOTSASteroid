using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;
using Random = UnityEngine.Random;

// [UpdateAfter(typeof(EndFramePhysicsSystem))]
public partial class OnTriggerSystem : SystemBase
{
    private BuildPhysicsWorld buildPhysicsWorld;
    private StepPhysicsWorld stepPhysicsWorld;
    public static ComponentDataFromEntity<PowerUpData> allPowerUps;
    public static ComponentDataFromEntity<PlayerTag> allPlayers;
    public static ComponentDataFromEntity<AsteroidData> allAsteroids;
    public static ComponentDataFromEntity<BulletData> allBullets;
    public static ComponentDataFromEntity<UFOData> allUFOs;
    // public static ComponentDataFromEntity<Translation> allTranslations;

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
            EntityCommandBuffer entityCommandBuffer = GameStateSystem.commandBufferSystem.CreateCommandBuffer();
            
            //TODO find a cleaner way to check for triggers
            #region Bullets and Asteroids triggers
            // Bullet entity A collides with asteroid Entity B
            if (allBullets.HasComponent(entityA) && allAsteroids.HasComponent(entityB))
            {
                KillEntity(entityCommandBuffer, entityA, entityB);
            }else if (allAsteroids.HasComponent(entityA) && allBullets.HasComponent(entityB)) // Bullet entity B collides with asteroid Entity A
            {
                KillEntity(entityCommandBuffer, entityB, entityA);
            }
            #endregion

            #region Player and Asteroids triggers
            if (allPlayers.HasComponent(entityA) && allAsteroids.HasComponent(entityB))
            {
                if (!GameManager.Instance.SpaceshipHasShield)
                {
                    if (!GameManager.Instance.PlayerRespawning)
                    {
                        GameManager.Instance.PlayerRespawning = true;
                        GameManager.Instance.PlayerLives--;
                    }
                }
                else
                {
                    entityCommandBuffer.DestroyEntity(entityB);
                    GameManager.Instance.SpaceshipHasShield = false;
                }
            }
            #endregion

            #region Player and PowerUps triggers

            if (allPowerUps.HasComponent(entityA) && allPlayers.HasComponent(entityB))
            {
                // Get power up type
                if (allPowerUps[entityA].PowerUpType == PowerUpData.PowerUpTypeEnum.Shield)
                {
                    GameManager.Instance.SpaceshipHasShield = true;
                }
                else
                {
                    GameManager.Instance.SpreadShotIsEnabled = true;
                }

                entityCommandBuffer.DestroyEntity(entityA);
            }
            #endregion

            #region Bullets and UFO
            if (allUFOs.HasComponent(entityA) && allBullets.HasComponent(entityB))
            {
                UFOData defaultUFOData = allUFOs[entityA];
                UFOData newUFOData = defaultUFOData;
                newUFOData.ResetUFOData = true;
                entityCommandBuffer.SetComponent(entityA, newUFOData);
                Entity vfxFClone = entityCommandBuffer.Instantiate(allUFOs[entityA].DamageVFX);
                Translation position = new Translation();
                position.Value = allUFOs[entityA].LastKnownTranslation.Value;
                entityCommandBuffer.SetComponent(vfxFClone, position);
            }else if (allBullets.HasComponent(entityA) && allUFOs.HasComponent(entityB))
            {
                UFOData defaultUFOData = allUFOs[entityB];
                UFOData newUFOData = defaultUFOData;
                newUFOData.ResetUFOData = true;
                entityCommandBuffer.SetComponent(entityB, newUFOData);
                Entity vfxFClone = entityCommandBuffer.Instantiate(allUFOs[entityB].DamageVFX);
                Translation position = new Translation();
                position.Value = allUFOs[entityB].LastKnownTranslation.Value;
                entityCommandBuffer.SetComponent(vfxFClone, position);
            }
            #endregion

            #region UFO Bullets with spaceship
            if (allPlayers.HasComponent(entityA) && allBullets.HasComponent(entityB))
            {
                Debug.Log("Player is Entity A");
            }else if (allBullets.HasComponent(entityA) && allPlayers.HasComponent(entityB))
            {
                if (!GameManager.Instance.PlayerRespawning)
                {
                    GameManager.Instance.PlayerRespawning = true;
                    GameManager.Instance.PlayerLives--;
                }
            }
            #endregion
        }

        private static void KillEntity(EntityCommandBuffer entityCommandBuffer, Entity attackerEntity, Entity damagedEntity)
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

            AsteroidData destroyedAsteroidData = allAsteroids[damagedEntity];
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

            Entity explosionVFX = entityCommandBuffer.Instantiate(allAsteroids[damagedEntity].ExplosionVfx);
            Translation position = new Translation
            {
                Value = destroyedAsteroidData.LastKnownTranslation.Value //Workaround since I was getting a Write only error accessing directly to translation data
            };
            entityCommandBuffer.SetComponent(explosionVFX, position);
            entityCommandBuffer.DestroyEntity(damagedEntity);
            GameManager.Instance.Score++;
        }
    }

    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        this.RegisterPhysicsRuntimeSystemReadOnly();
    }
    
    protected override void OnUpdate()
    {
        allPowerUps = GetComponentDataFromEntity<PowerUpData>(true);
        allPlayers = GetComponentDataFromEntity<PlayerTag>(true);
        allAsteroids = GetComponentDataFromEntity<AsteroidData>();
        allUFOs = GetComponentDataFromEntity<UFOData>(true);
        allBullets = GetComponentDataFromEntity<BulletData>(true);
        // allTranslations = GetComponentDataFromEntity<Translation>();
        
        Dependency.Complete();
    
        NativeList<TriggerEvent> triggerEvents = new NativeList<TriggerEvent>(Allocator.TempJob);
    
        var collectTriggerEventsJob = new CollectTriggerEventsJob
        {
            triggerEvents = triggerEvents
        };
    
        var handle = collectTriggerEventsJob.Schedule(stepPhysicsWorld.Simulation, Dependency);
        handle.Complete();
    
        var physicsWorld = buildPhysicsWorld.PhysicsWorld;
        int expectedNumberOfTriggerEvents = 0;
        
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
    
                // bool isTrigger = false;
                // unsafe
                // {
                //     ConvexCollider* colliderPtr = (ConvexCollider*)triggerBody.Collider.GetUnsafePtr();
                //     var material = colliderPtr->Material;
                //
                //     isTrigger = colliderPtr->Material.CollisionResponse == CollisionResponsePolicy.RaiseTriggerEvents;
                // }
                //
                // float distance = math.distance(triggerBody.WorldFromBody.pos, nonTriggerBody.WorldFromBody.pos);
                
            }).Run();

        triggerEvents.Dispose();
    }

}
