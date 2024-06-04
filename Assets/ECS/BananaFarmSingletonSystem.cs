using Unity.Collections;
using Unity.Entities;

public partial struct NumBananasFarmedSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SingletonAuthor.BananaFarmNumToSpawn>();
        state.RequireForUpdate<SingletonAuthor.BananaFarmPrefab>();
        // Ensure the singleton entity with the SingletonInt component exists
        
    }
    public void OnUpdate(ref SystemState state)
    {
        var comp = SystemAPI.GetSingleton<SingletonAuthor.BananaFarmNumToSpawn>();
        int numToSpawn = comp.NumToSpawn;
        
        
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);

        if (numToSpawn > 0)
            do
                ecb.Instantiate(SystemAPI.GetSingleton<SingletonAuthor.BananaFarmPrefab>().Entity);
            while (--numToSpawn > 0);

        if (numToSpawn < 0)
        {

            int index = -1;
            var x = state.EntityManager.CreateEntityQuery(typeof(BananaAuthor.IsBananaFarm)).ToEntityArray(Allocator.TempJob);
            do
            {
                if (++index >= x.Length) break;
                ecb.DestroyEntity(x[index]);
            } while (++numToSpawn < 0);
        }

        SystemAPI.SetSingleton(new SingletonAuthor.BananaFarmNumToSpawn()
        {
            NumToSpawn = 0
        });
        ecb.Playback(state.EntityManager);
        ecb.Dispose();

    }
}
