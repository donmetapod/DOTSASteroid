using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

public class AsteroidSpawnerMono : MonoBehaviour
{
    public GameObject Prefab;
    [SerializeField] private int asteroidInstatiationDelay = 3;
    [SerializeField] private int createdAsteroids;
    [SerializeField] private List<Entity> entitiesOnScene = new List<Entity>();
    
    IEnumerator Start()
    {
        BlobAssetStore blob = new BlobAssetStore();
        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blob);
        // var prefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(Prefab, settings);
        // var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        while (true)
        {

            EntityCommandBuffer entityCommandBuffer = GameStateSystem.commandBufferSystem.CreateCommandBuffer();
            Entity asteroidEntity = entityCommandBuffer.Instantiate(GameObjectConversionUtility.ConvertGameObjectHierarchy(Prefab, settings));//entityManager.Instantiate(prefab);
            Vector2 spawnArea = new Vector2(Random.Range(20, 25), Random.Range(20, 25));
            int swapXPossibility = Random.Range(1, 100);
            if (swapXPossibility < 50) { spawnArea.x *= -1; }
            int swapYPossibility = Random.Range(1, 100);
            if (swapYPossibility < 50) { spawnArea.y *= -1; }
            Translation asteroidInitialTranslation = new Translation();
            asteroidInitialTranslation.Value = new float3(spawnArea.x, spawnArea.y, 0);
            entityCommandBuffer.SetComponent(asteroidEntity, asteroidInitialTranslation);
            entitiesOnScene.Add(asteroidEntity);
            createdAsteroids++;
            yield return new WaitForSeconds(asteroidInstatiationDelay);
        }
    }
}
