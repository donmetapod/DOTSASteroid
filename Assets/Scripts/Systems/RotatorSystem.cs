using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public partial class RotatorSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        Entities.ForEach((ref Translation translation, ref Rotation rotation, in EntityRotatorData rotatorData) =>
        {
            rotation.Value = math.mul(rotation.Value, quaternion.RotateX(math.radians(rotatorData.XRotationSpeed * deltaTime)));
            rotation.Value = math.mul(rotation.Value, quaternion.RotateY(math.radians(rotatorData.YRotationSpeed * deltaTime)));
            rotation.Value = math.mul(rotation.Value, quaternion.RotateZ(math.radians(rotatorData.ZRotationSpeed * deltaTime)));
        }).Schedule();
    }
}
