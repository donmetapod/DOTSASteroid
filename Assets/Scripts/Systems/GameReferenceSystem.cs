using Unity.Entities;

public partial class GameReferenceSystem : SystemBase
{
    public static EndSimulationEntityCommandBufferSystem CommandBufferSystem;
    public static ComponentDataFromEntity<SpaceshipData> AllPlayers;
    public static ComponentDataFromEntity<PowerUpData> allPowerUps;
    public static ComponentDataFromEntity<AsteroidData> allAsteroids;
    public static ComponentDataFromEntity<BulletData> allBullets;
    public static ComponentDataFromEntity<UFOData> allUFOs;
    protected override void OnCreate()
    {
        base.OnCreate();
        CommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        AllPlayers = GetComponentDataFromEntity<SpaceshipData>(true);
        allPowerUps = GetComponentDataFromEntity<PowerUpData>(true);
        allAsteroids = GetComponentDataFromEntity<AsteroidData>();
        allUFOs = GetComponentDataFromEntity<UFOData>(true);
        allBullets = GetComponentDataFromEntity<BulletData>(true);
    }
}
