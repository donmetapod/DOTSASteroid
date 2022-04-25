using Unity.Entities;

[GenerateAuthoringComponent]
public struct SpawnData : IComponentData
{
    public Entity EntityToSpawn;
    public float SpawnDelayTime;
    public float SpawnAreaCloseRange;
    public float SpawnAreaFarRange;
}
