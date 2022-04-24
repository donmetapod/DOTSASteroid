using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


[GenerateAuthoringComponent]
public struct BulletData : IComponentData
{
    public float Speed;
    public bool IsActive;
    public bool AddedToPool;
    public bool UFOBullet;
    public bool Collided;
}
