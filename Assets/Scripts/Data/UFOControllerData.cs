using Unity.Entities;

[GenerateAuthoringComponent]
public struct UFOControllerData : IComponentData
{
    public Entity UFO;
    public Entity SmallUFO;
}
