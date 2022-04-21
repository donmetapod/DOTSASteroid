using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

public struct AsteroidTrigger : IComponentData
{
    public float Test;
}

public class TriggerReactionAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public float Test = 1;
    void IConvertGameObjectToEntity.Convert(Entity entity, EntityManager entityManager, GameObjectConversionSystem conversionSystem)
    {
        if (enabled)
        {
            entityManager.AddComponentData(entity, new AsteroidTrigger()
            {
                Test = Test
            });
        }
    }
}

// // System
// [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
// [UpdateAfter(typeof(ExportPhysicsWorld))]
// [UpdateBefore(typeof(EndFramePhysicsSystem))]
// public partial class AsteroidTriggerReaction : SystemBase
// {
//     private StepPhysicsWorld _stepPhysicsWorld;
//     private EntityQuery _entityQuery;
//
//     protected override void OnCreate()
//     {
//         _stepPhysicsWorld = World.GetExistingSystem<StepPhysicsWorld>();
//         _entityQuery = GetEntityQuery(new EntityQueryDesc
//         {
//             All = new ComponentType[]
//             {
//                 typeof(AsteroidTrigger)
//             }
//         });
//     }
//
//     [BurstCompile]
//     struct AsteroidTriggerReactionJob : ITriggerEventsJob
//     {
//         [ReadOnly]public ComponentDataFromEntity<AsteroidTrigger> AsteroidGroup;
//         public ComponentDataFromEntity<PhysicsGravityFactor> PhysicsGravityFactorGroup;
//         public ComponentDataFromEntity<PhysicsVelocity> PhysiscsVelocityGroup;
//         public void Execute(TriggerEvent triggerEvent)
//         {
//             Entity entityA = triggerEvent.EntityA;
//             Entity entityB = triggerEvent.EntityB;
//             Debug.Log(entityA.Index);
//             Debug.Log(entityB.Index);
//             bool isEntityADynamic = PhysiscsVelocityGroup.HasComponent(entityA);
//             bool isEntityBDynamic = PhysiscsVelocityGroup.HasComponent(entityB);
//             if (!isEntityADynamic && !isEntityBDynamic)
//             {
//                 return;
//             }
//         
//             // EntityManager entityManager = new EntityManager();
//             // entityManager.DestroyEntity(entityA);
//             // entityManager.DestroyEntity(entityB);
//             // var triggerComponent = AsteroidGroup[entityA];
//             // {
//             //     var component = PhysiscsVelocityGroup[entityA];
//             //     component.
//             // }
//
//         }
//     }
//
//     protected override void OnStartRunning()
//     {
//         base.OnStartRunning();
//         this.RegisterPhysicsRuntimeSystemReadOnly();
//     }
//
//     protected override void OnUpdate()
//     {
//         if (_entityQuery.CalculateEntityCount() == 0)
//         {
//             return;
//         }
//
//         Dependency = new AsteroidTriggerReactionJob
//         {
//             AsteroidGroup = GetComponentDataFromEntity<AsteroidTrigger>(true),
//             PhysicsGravityFactorGroup = GetComponentDataFromEntity<PhysicsGravityFactor>(),
//             PhysiscsVelocityGroup = GetComponentDataFromEntity<PhysicsVelocity>()
//
//         }.Schedule(_stepPhysicsWorld.Simulation, Dependency);
//     }
// }
