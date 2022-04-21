using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct WarpLimitsData : IComponentData
{
    public float ScreenXLimit;
    public float ScreenYLimit;
}
