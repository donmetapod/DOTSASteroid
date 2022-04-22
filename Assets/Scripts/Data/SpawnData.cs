using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct SpawnData : IComponentData
{
    public Entity EntityToSpawn;
    public float SpawnDelayTime;
    public float SpawnAreaCloseRange;
    public float SpawnAreaFarRange;
}
