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
    public static ComponentDataFromEntity<PowerUpTag> allPowerUps;
    public static ComponentDataFromEntity<PlayerTag> allPlayers;
    public static ComponentDataFromEntity<AsteroidData> allAsteroids;
    public static ComponentDataFromEntity<BulletData> allBullets;
    public static EndSimulationEntityCommandBufferSystem commandBufferSystem;

    private EntityQuery entityQuery;
    
    protected override void OnCreate()
    {
        buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
        commandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        
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
            EntityCommandBuffer entityCommandBuffer = commandBufferSystem.CreateCommandBuffer();
            
            if (allAsteroids.HasComponent(entityA) && allAsteroids.HasComponent(entityB))
            {
                return;
            }

            // Bullet entity A collides with asteroid Entity B
            if (allBullets.HasComponent(entityA) && allAsteroids.HasComponent(entityB))
            {
                BulletData inactiveBullet = new BulletData
                {
                    IsActive = false,
                    Speed = 0
                };
                entityCommandBuffer.SetComponent(entityA, inactiveBullet);
                Translation defaultBulletTranslation = new Translation();
                defaultBulletTranslation.Value = new float3(100, 100, 100);
                entityCommandBuffer.SetComponent(entityA, defaultBulletTranslation);

                AsteroidData destroyedAsteroidData = allAsteroids[entityB];
                if (!destroyedAsteroidData.IsSmallAsteroid)
                {
                    for (int i = 0; i < 2; i++)// Create two small asteroids when a big one is destroyed
                    {
                        Entity smallAsteroidClone = entityCommandBuffer.Instantiate(destroyedAsteroidData.SmallerAsteroid);
                        Translation smallAsteroidTranslation = destroyedAsteroidData.LastKnownTranslation;
                        smallAsteroidTranslation.Value.x += i;
                        smallAsteroidTranslation.Value.y += i;
                        entityCommandBuffer.SetComponent(smallAsteroidClone, smallAsteroidTranslation);
                    }
                   
                }
                entityCommandBuffer.DestroyEntity(entityB);
            }else if (allAsteroids.HasComponent(entityA) && allBullets.HasComponent(entityB)) // Bullet entity B collides with asteroid Entity A
            {
                entityCommandBuffer.DestroyEntity(entityA);
                BulletData inactiveBullet = new BulletData
                {
                    IsActive = false,
                    Speed = 0
                };
                entityCommandBuffer.SetComponent(entityB, inactiveBullet);
                Translation defaultBulletTranslation = new Translation();
                defaultBulletTranslation.Value = new float3(100, 100, 100);
                entityCommandBuffer.SetComponent(entityB, defaultBulletTranslation);
            }

            if (allAsteroids.HasComponent(entityA) && allPlayers.HasComponent(entityB))
            {
                // Debug.Log("Power up Entity A: " + entityA + " Collided with Player Entity B: " + entityB);
            }else if (allPlayers.HasComponent(entityA) && allAsteroids.HasComponent(entityB))
            {
                // Destroy Player when touched by asteroid
                entityCommandBuffer.DestroyEntity(entityA);
            }
        }
    }

    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        this.RegisterPhysicsRuntimeSystemReadOnly();
    }

   
    protected override void OnUpdate()
    {
        allPowerUps = GetComponentDataFromEntity<PowerUpTag>(true);
        allPlayers = GetComponentDataFromEntity<PlayerTag>(true);
        allAsteroids = GetComponentDataFromEntity<AsteroidData>(true);
        allBullets = GetComponentDataFromEntity<BulletData>(true);
        
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
                // expectedNumberOfTriggerEvents += eventChecker.NumExpectedEvents;
    
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
    
                bool isTrigger = false;
                unsafe
                {
                    ConvexCollider* colliderPtr = (ConvexCollider*)triggerBody.Collider.GetUnsafePtr();
                    var material = colliderPtr->Material;
    
                    isTrigger = colliderPtr->Material.CollisionResponse == CollisionResponsePolicy.RaiseTriggerEvents;
                }

                float distance = math.distance(triggerBody.WorldFromBody.pos, nonTriggerBody.WorldFromBody.pos);
                
            }).Run();

        triggerEvents.Dispose();
    }

}
