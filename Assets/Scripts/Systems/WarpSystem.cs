using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public partial class WarpSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref Translation position, in WarpLimitsData screenLimitsData) => {
            // Warp X Position
            if (position.Value.x > screenLimitsData.ScreenXLimit) {
                position.Value.x = -screenLimitsData.ScreenXLimit;
            }
            if (position.Value.x < -screenLimitsData.ScreenXLimit)
            {
                position.Value.x = screenLimitsData.ScreenXLimit;
            }
            // Warp Y Position
            if (position.Value.y > screenLimitsData.ScreenYLimit)
            {
                position.Value.y = -screenLimitsData.ScreenYLimit;
            }
            if (position.Value.y < -screenLimitsData.ScreenYLimit)
            {
                position.Value.y = screenLimitsData.ScreenYLimit;
            }
        }).Schedule();
    }
}
