using Unity.Entities;

[GenerateAuthoringComponent]
public struct PowerUpData : IComponentData
{
    public enum PowerUpTypeEnum
    {
        Shield,
        SpreadShot
    }

    public PowerUpTypeEnum PowerUpType;

}
