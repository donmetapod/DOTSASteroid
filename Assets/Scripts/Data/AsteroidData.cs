using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[GenerateAuthoringComponent]
public struct AsteroidData : IComponentData
{
    public bool IsSmallAsteroid;
    public float MovementSpeed;
    public float3 MoveDirection;
    public float TargetOffset;
    public Entity SmallerAsteroid;
    public Entity ExplosionVfx;
    public Translation LastKnownTranslation;
    public Rotation LastKnownRotation;
}
