using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct SelfDestroyData : IComponentData
{
    public float LifeTime;
    public bool DestroyTimeSet;
    public float DestroyTime;
}
