using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public partial class UFOControllerSystem : SystemBase
{
    private float gameTime = 0;

    protected override void OnCreate()
    {
        Debug.Log("on create");
        Entities.ForEach((in UFOControllerData ufoControllerData) => {
            EntityManager.SetEnabled(ufoControllerData.SmallUFO, false);
        }).WithStructuralChanges().Run(); 
    }

    protected override void OnUpdate()
    {
        gameTime += Time.DeltaTime;
        
        Entities.ForEach((in UFOControllerData ufoControllerData) => {
            if (gameTime > 20 && !EntityManager.GetEnabled(ufoControllerData.UFO))
            {
                if(EntityManager.GetEnabled(ufoControllerData.UFO))
                    EntityManager.SetEnabled(ufoControllerData.UFO, false);
                
                EntityManager.SetEnabled(ufoControllerData.SmallUFO, true);
            }
        }).WithStructuralChanges().Run(); 
    }
}
