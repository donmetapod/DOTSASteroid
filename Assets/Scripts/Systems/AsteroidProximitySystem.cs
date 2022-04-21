using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class AsteroidProximitySystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem commandBufferSystem;

    protected override void OnCreate()
    {
        commandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        // // Get player Entity
        // EntityQuery playerQuery = GetEntityQuery(ComponentType.ReadOnly<Ship>());
        // if(playerQuery.IsEmpty)
        //     return;
        //
        // Entity playerEntity = playerQuery.ToEntityArray(Allocator.Temp)[0];
        // // Entity playerEntity = playerQuery.GetSingletonEntity();
        // EntityCommandBuffer entityCommandBuffer = commandBufferSystem.CreateCommandBuffer();
        //
        // Entities.
        //     WithAll<AsteroidTag>().
        //     ForEach((ref AsteroidData asteroid, in Translation translation, in Rotation rotation) =>
        //     {
        //         ComponentDataFromEntity<Translation> positions = GetComponentDataFromEntity<Translation>(true);
        //         
        //         float distanceToPlayer = math.distancesq(translation.Value, positions[playerEntity].Value);
        //         if (distanceToPlayer < 1)
        //         {
        //             entityCommandBuffer.DestroyEntity(asteroid.Myself);
        //         }
        //     }).Run();
    }
}
