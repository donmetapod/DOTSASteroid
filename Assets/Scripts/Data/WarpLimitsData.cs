using Unity.Entities;

[GenerateAuthoringComponent]
public struct WarpLimitsData : IComponentData
{
    public float ScreenXLimit;
    public float ScreenYLimit;
}
