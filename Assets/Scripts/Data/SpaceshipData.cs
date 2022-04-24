using JetBrains.Annotations;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[GenerateAuthoringComponent]
public struct SpaceshipData : IComponentData
{
    public float3 Direction;
    public float MovementForce;
    public float TurnSpeed;
    public Entity Shield;
    public Entity SpreadShot;
    public Entity DestroyVfx;
    public Translation LastKnowPosition;
}


