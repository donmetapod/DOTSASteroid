using Unity.Entities;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

[GenerateAuthoringComponent]
public struct UFOData : IComponentData
{
    public bool InAction;
    public bool alreadyChangedDirecion;
    public bool ShotByPlayer;
    public bool ResetUFOData;
    public float3 MoveDirection;
    public float MovementSpeed;
    public float StartActionDelayTime;
    public float StartActionAccumulatedTime;
    public float DirectionChangeTime;
    public float DirectionChangeAccumulatedTime;
    public float YScreenLimit;
    public Random RandomValue;
    public float GameTime; // Used for random seed
    public float DistanceFromCenterResetValue;
    public float FireRate;
    public float ShootAccumulationTime;
}
