using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

//Not using it for now, currently testing mono approach just for fun
public partial class SpawnSystem : SystemBase
{
    // private float accumulatedTime;
    // private EntityCommandBuffer entityCommandBuffer;
    //
    // protected override void OnCreate()
    // {
    //     base.OnCreate();
    //     entityCommandBuffer = GameStateSystem.commandBufferSystem.CreateCommandBuffer();
    // }

    protected override void OnUpdate()
    {
        // accumulatedTime += Time.DeltaTime;
        // Entities.ForEach((in SpawnData spawnData) =>
        // {
        //     if (accumulatedTime > spawnData.SpawnDelayTime)
        //     {
        //         Entity clone = entityCommandBuffer.Instantiate(spawnData.EntityToSpawn);
        //         // Entity clone = EntityManager.Instantiate(spawnData.EntityToSpawn);
        //         Vector2 spawnArea = new Vector2(Random.Range(spawnData.SpawnAreaCloseRange, spawnData.SpawnAreaFarRange)
        //             , Random.Range(spawnData.SpawnAreaCloseRange, spawnData.SpawnAreaFarRange));
        //         int swapXPossibility = Random.Range(1, 100);
        //         if (swapXPossibility < 50) { spawnArea.x *= -1; }
        //         int swapYPossibility = Random.Range(1, 100);
        //         if (swapYPossibility < 50) { spawnArea.y *= -1; }
        //         Translation clonInitialTranslation = new Translation();
        //         clonInitialTranslation.Value = new float3(spawnArea.x, spawnArea.y, 0);
        //         EntityManager.SetComponentData(clone, clonInitialTranslation);
        //         accumulatedTime = 0;
        //     }
        // }).WithStructuralChanges().Run();
    }
}
