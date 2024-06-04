using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Entities;
using UnityEngine;

namespace ECS
{
    [BurstCompile]
    public partial struct BananaFarmSystemJobChunkCombined : ISystem
    {
        private EntityQuery _query;
        private ComponentTypeHandle<BananaAuthor.GroupedTimeToSpawn> _groupedTimeToSpawn;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _query = SystemAPI.QueryBuilder().WithAllRW<BananaAuthor.GroupedTimeToSpawn>().Build();

            _groupedTimeToSpawn = state.GetComponentTypeHandle<BananaAuthor.GroupedTimeToSpawn>(false);
            state.RequireForUpdate(_query);
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<SingletonAuthor.NumBananasFarmed>();
            state.RequireForUpdate<SingletonAuthor.ChunkFarmer>();

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecbSystem = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSystem.CreateCommandBuffer(state.WorldUnmanaged);

            _groupedTimeToSpawn.Update(ref state);

            float deltaTime = SystemAPI.Time.DeltaTime;

            var job = new BananaFarmSystemChunkJobCombined
            {
                DeltaTime = deltaTime,
                ComponentHandle = _groupedTimeToSpawn,
                NumBananasFarmed =  SystemAPI.GetSingleton<SingletonAuthor.NumBananasFarmed>().Value,
                SingletonEntity =  SystemAPI.GetSingletonEntity<SingletonAuthor.NumBananasFarmed>(),
                ECB = ecb.AsParallelWriter()
            };

            state.Dependency = job.ScheduleParallel(_query, state.Dependency);
        }
    }

    [BurstCompile]
    public struct BananaFarmSystemChunkJobCombined : IJobChunk
    {
        public float DeltaTime;
        public ComponentTypeHandle<BananaAuthor.GroupedTimeToSpawn> ComponentHandle;
        public ulong NumBananasFarmed;
        public Entity SingletonEntity;
        public EntityCommandBuffer.ParallelWriter ECB;

        [BurstCompile]
        public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
        {
            var timers = chunk.GetNativeArray(ref ComponentHandle);

            
            for (int i = 0; i < chunk.Count; i++)
            {
                var current = timers[i];
                current.CurrentTime += DeltaTime;
                timers[i] = current;

                if (current.CurrentTime >= current.TimeToSpawn)
                {
                    current.CurrentTime = 0;
                    timers[i] = current;
                    NumBananasFarmed++;

                }
            }
            
            ECB.SetComponent(unfilteredChunkIndex, SingletonEntity, new SingletonAuthor.NumBananasFarmed { Value = NumBananasFarmed });
        }

    }
}
