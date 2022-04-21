using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct SpaceshipData : IComponentData
{
    public float3 Direction;
    public float MovementForce;
    public float TurnSpeed;
}


