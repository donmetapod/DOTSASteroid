using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;


[GenerateAuthoringComponent]
[NativeContainerSupportsMinMaxWriteRestriction]
public struct BulletData : IComponentData
{
    public float Speed;
    public bool IsActive;
    public bool AddedToPool;
    public bool UFOBullet;
    public bool Collided;
}
