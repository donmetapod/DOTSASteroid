using Unity.Entities;
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
                entityCommandBuffer = GameReferenceSystem.CommandBufferSystem.CreateCommandBuffer();
                entityCommandBuffer.DestroyEntity(entity);
            }
        }).WithStructuralChanges().Run();
    }
}
