using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct EntityRotatorData : IComponentData
{
    public float XRotationSpeed;
    public float YRotationSpeed;
    public float ZRotationSpeed;

}
