using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

[GenerateAuthoringComponent]
public struct UFOData : IComponentData
{
    public bool InAction;
    // public bool IsSmallUFO;
    // public bool ActiveUFO;
    public bool AlreadyChangedDirecion;
    public bool ResetUFOData;
    public float FirstAttackDelayTime;
    public float3 MoveDirection;
    public float MovementSpeed;
    public float StartActionDelayTime;
    public float StartActionAccumulatedTime;
    public float DirectionChangeTime;
    public float DirectionChangeAccumulatedTime;
    public float YScreenLimit;
    public Random RandomValue;
    public float GameTime;
    public float DistanceFromCenterResetValue;
    public float FireRate;
    public float ShootAccumulationTime;
    public Entity DamageVFX; 
    public Translation LastKnownTranslation;
}
