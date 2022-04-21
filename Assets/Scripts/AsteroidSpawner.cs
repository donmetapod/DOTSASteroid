using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
    public GameObject AsteroidPrefab;
    public int CountX;
    public int CountY;
    
    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(AsteroidPrefab);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var spawnerData = new AsteroidSpawnerFromEntity
        {
            AsteroidPrefab = conversionSystem.GetPrimaryEntity(AsteroidPrefab),
            CountX = CountX,
            CountY = CountY
        };
        dstManager.AddComponentData(entity, spawnerData);
    }
}

public struct AsteroidSpawnerFromEntity : IComponentData
{
    public Entity AsteroidPrefab;
    public int CountX;
    public int CountY;
}
