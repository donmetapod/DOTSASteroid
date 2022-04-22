using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;
using FixedStepSimulationSystemGroup = Unity.Entities.FixedStepSimulationSystemGroup;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(ExportPhysicsWorld))]
[UpdateBefore(typeof(EndFramePhysicsSystem))]
public partial class AsteroidTriggerReaction : SystemBase
{
    private StepPhysicsWorld _stepPhysicsWorld;
    private EntityQuery _entityQuery;

    protected override void OnCreate()
    {
        _stepPhysicsWorld = World.GetExistingSystem<StepPhysicsWorld>();
    }

    [BurstCompile]
    struct AsteroidTriggerReactionJob : ITriggerEventsJob
    {
        [ReadOnly]public ComponentDataFromEntity<AsteroidTrigger> AsteroidGroup;
        public ComponentDataFromEntity<PhysicsGravityFactor> PhysicsGravityFactorGroup;
        public ComponentDataFromEntity<PhysicsVelocity> PhysiscsVelocityGroup;
        public void Execute(TriggerEvent triggerEvent)
        {
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;

            bool isEntityADynamic = PhysiscsVelocityGroup.HasComponent(entityA);
            bool isEntityBDynamic = PhysiscsVelocityGroup.HasComponent(entityB);
            if (!isEntityADynamic && !isEntityBDynamic)
            {
                return;
            }
        
            EntityManager entityManager = new EntityManager();
            if(entityA != null)
                entityManager.DestroyEntity(entityA);

        }
    }

    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        this.RegisterPhysicsRuntimeSystemReadOnly();
    }

    protected override void OnUpdate()
    {
        _entityQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[]
            {
                typeof(AsteroidTrigger)
            }
        });
        
        if (_entityQuery.CalculateEntityCount() == 0)
        {
            return;
        }

        Dependency = new AsteroidTriggerReactionJob
        {
            AsteroidGroup = GetComponentDataFromEntity<AsteroidTrigger>(true),
            PhysicsGravityFactorGroup = GetComponentDataFromEntity<PhysicsGravityFactor>(),
            PhysiscsVelocityGroup = GetComponentDataFromEntity<PhysicsVelocity>()

        }.Schedule(_stepPhysicsWorld.Simulation, Dependency);
    }
}
