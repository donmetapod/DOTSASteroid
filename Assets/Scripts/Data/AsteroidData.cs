using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[GenerateAuthoringComponent]
public struct AsteroidData : IComponentData
{
    public bool IsSmallAsteroid;
    public float MovementSpeed;
    public float3 MoveDirection;
    public Entity SmallerAsteroid;
    public Translation LastKnownTranslation;
    public bool MarkedForDestroy;
}
