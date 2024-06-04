using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[UpdateAfter(typeof(TransformSystemGroup))]
[BurstCompile]
public partial struct BananaFarmSystemIdiomatic : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SingletonAuthor.NumBananasFarmed>();
        state.RequireForUpdate<SingletonAuthor.IdiomaticFarmer>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        
        var value = SystemAPI.GetSingleton<SingletonAuthor.NumBananasFarmed>().Value;

        
        foreach (var groupedTimeToSpawn in SystemAPI.Query<RefRW<BananaAuthor.GroupedTimeToSpawn>>())
        {
            groupedTimeToSpawn.ValueRW.CurrentTime += deltaTime;
            
            if (groupedTimeToSpawn.ValueRW.CurrentTime >= groupedTimeToSpawn.ValueRW.TimeToSpawn)
            {
                groupedTimeToSpawn.ValueRW.CurrentTime = 0;
                 ++value;
            }
        }
        
        foreach (var (timeToSpawn, currentTime) in SystemAPI.Query<RefRO<BananaAuthor.TimeToSpawnComponent>, RefRW<BananaAuthor.CurrentTimeToSpawnComponent>>())
        {
            currentTime.ValueRW.CurrentTime += deltaTime;

            if (currentTime.ValueRW.CurrentTime >= timeToSpawn.ValueRO.TimeToSpawn)
            {
                currentTime.ValueRW.CurrentTime = 0;
                ++value;
            }
        }
        
        foreach (var (timeToSpawn, currentTime) in SystemAPI.Query<RefRO<BananaAuthor.TimeToSpawnComponentBlob>, RefRW<BananaAuthor.CurrentTimeToSpawnComponent>>())
        {
            currentTime.ValueRW.CurrentTime += deltaTime;

            if (currentTime.ValueRW.CurrentTime >= timeToSpawn.ValueRO.Blob.Value.TimeToSpawn)
            {
                currentTime.ValueRW.CurrentTime = 0;
                ++value;
            }
        }
        
        /*
        //This method is disabled because it'll run always, and I'm too lazy to do it properly
        foreach (var currentTime in SystemAPI.Query<RefRW<BananaAuthor.CurrentTimeToSpawnComponent>>())
        {
            currentTime.ValueRW.CurrentTime += deltaTime;

            if (currentTime.ValueRW.CurrentTime >= 0.2f)
            {
                currentTime.ValueRW.CurrentTime = 0;
                ++value;
            }
        }
        */
        
        SystemAPI.SetSingleton(new SingletonAuthor.NumBananasFarmed
        {
            Value = value
        });

    }
}
