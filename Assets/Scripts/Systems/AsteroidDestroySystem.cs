using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(OnTriggerSystem))]
public partial class AsteroidDestroySystem : SystemBase
{
    EntityCommandBuffer entityCommandBuffer;

    protected override void OnCreate()
    {
        base.OnCreate();
        // entityCommandBuffer = OnTriggerSystem.commandBufferSystem.CreateCommandBuffer();
    }

    protected override void OnUpdate()
    {
        // Entities.ForEach((Entity entity, in AsteroidData asteroidData) => {
        //     if (asteroidData.MarkedForDestroy)
        //     {
        //         Debug.Log("destroy asteroid and add score");
        //         entityCommandBuffer.DestroyEntity(entity);
        //     }
        // }).WithoutBurst().Run();
    }
}
