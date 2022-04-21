using Unity.Entities;

[GenerateAuthoringComponent]
public struct EntityTargetData : IComponentData
{
    public Entity target;
}
