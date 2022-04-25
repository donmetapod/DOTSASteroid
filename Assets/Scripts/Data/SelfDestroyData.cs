using Unity.Entities;

[GenerateAuthoringComponent]
public struct SelfDestroyData : IComponentData
{
    public float LifeTime;
    public bool DestroyTimeSet;
    public float DestroyTime;
}
