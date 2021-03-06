using System.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

//Testing Entity instantiation from Monobehaviour
public class SpawnerMono : MonoBehaviour
{
    public GameObject[] Prefab;
    [SerializeField] private bool graduallyReduceSpawnDelay;
    [SerializeField] private float reduceSpawnDelayRatio = 0.01f;
    [SerializeField] private float initialSpawnDelay = 10;
    [SerializeField] private float instatiationDelay = 3;
    [SerializeField] private float spawnAreaCloseRange = 20;
    [SerializeField] private float spawnAreaFarRange = 25;

    IEnumerator Start()
    {
        
        BlobAssetStore blob = new BlobAssetStore();
        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blob);
        
        yield return new WaitForSeconds(initialSpawnDelay);
        while (true)
        {
            int randomSpawnObj = Random.Range(0, Prefab.Length);
            Entity prefabGOToEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(Prefab[randomSpawnObj], settings);
            EntityCommandBuffer entityCommandBuffer = GameReferenceSystem.CommandBufferSystem.CreateCommandBuffer();
            Entity clone = entityCommandBuffer.Instantiate(prefabGOToEntity);
            Vector2 spawnArea = new Vector2(Random.Range(spawnAreaCloseRange, spawnAreaFarRange), Random.Range(spawnAreaCloseRange, spawnAreaFarRange));
            int swapXPossibility = Random.Range(1, 100);
            if (swapXPossibility < 50) { spawnArea.x *= -1; }
            int swapYPossibility = Random.Range(1, 100);
            if (swapYPossibility < 50) { spawnArea.y *= -1; }
            Translation initialTranslation = new Translation();
            initialTranslation.Value = new float3(spawnArea.x, spawnArea.y, 0);
            entityCommandBuffer.SetComponent(clone, initialTranslation);
            yield return new WaitForSeconds(instatiationDelay);
            if (graduallyReduceSpawnDelay && instatiationDelay > 1)
            {
                instatiationDelay -= reduceSpawnDelayRatio;
            }
        }
    }
}
