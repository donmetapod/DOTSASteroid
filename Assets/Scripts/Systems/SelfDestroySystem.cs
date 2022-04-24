using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public partial class SelfDestroySystem : SystemBase
{
    private float gameTime;
    private EntityCommandBuffer entityCommandBuffer;
    protected override void OnUpdate()
    {
        gameTime += Time.DeltaTime;
        Entities.ForEach((Entity entity, ref SelfDestroyData selfDestroyData) => {
            if (!selfDestroyData.DestroyTimeSet)
            {
                selfDestroyData.DestroyTime = gameTime + selfDestroyData.LifeTime;
                selfDestroyData.DestroyTimeSet = true;
            }

            if (selfDestroyData.DestroyTime < gameTime)
            {
                entityCommandBuffer = GameStateSystem.commandBufferSystem.CreateCommandBuffer();
                entityCommandBuffer.DestroyEntity(entity);
            }
        }).WithStructuralChanges().Run();
    }
}
