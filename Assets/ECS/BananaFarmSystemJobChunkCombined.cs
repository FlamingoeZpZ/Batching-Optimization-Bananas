using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace ECS
{
    [BurstCompile]
    public partial struct BananaFarmSystemJobChunkCombined : ISystem
    {
        private EntityQuery _query;
        private ComponentTypeHandle<BananaAuthor.GroupedTimeToSpawn> _groupedTimeToSpawn;
        private NativeQueue<ulong> _farmResultsQueue;
        private NativeReference<ulong> _totalFarmed;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _query = SystemAPI.QueryBuilder().WithAllRW<BananaAuthor.GroupedTimeToSpawn>().Build();

            _groupedTimeToSpawn = state.GetComponentTypeHandle<BananaAuthor.GroupedTimeToSpawn>(false);
            _farmResultsQueue = new NativeQueue<ulong>(Allocator.Persistent);
            _totalFarmed = new NativeReference<ulong>(Allocator.Persistent);

            state.RequireForUpdate(_query);
            state.RequireForUpdate<SingletonAuthor.NumBananasFarmed>();
            state.RequireForUpdate<SingletonAuthor.ChunkFarmer>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            _farmResultsQueue.Dispose();
            _totalFarmed.Dispose();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _groupedTimeToSpawn.Update(ref state);

            float deltaTime = SystemAPI.Time.DeltaTime;

            // Clear the queue before starting new calculations
            _farmResultsQueue.Clear();
            _totalFarmed.Value = 0;

            var farmJob = new BananaFarmSystemChunkJobCombined
            {
                DeltaTime = deltaTime,
                ComponentHandle = _groupedTimeToSpawn,
                FarmResultsQueue = _farmResultsQueue.AsParallelWriter()
            };

            var handle = farmJob.ScheduleParallel(_query, state.Dependency);
            
            var sumJob = new SumFarmResultsJob
            {
                FarmResultsQueue = _farmResultsQueue,
                TotalFarmed = _totalFarmed
            };

            handle = sumJob.Schedule(handle);
            
            state.Dependency = handle;

            // Complete the job and update the singleton value
            state.Dependency.Complete();

            var numBananasFarmed = SystemAPI.GetSingleton<SingletonAuthor.NumBananasFarmed>().Value;
            numBananasFarmed += _totalFarmed.Value;

            SystemAPI.SetSingleton(new SingletonAuthor.NumBananasFarmed
            {
                Value = numBananasFarmed
            });
        }

        [BurstCompile]
        public struct SumFarmResultsJob : IJob
        {
            public NativeQueue<ulong> FarmResultsQueue;
            public NativeReference<ulong> TotalFarmed;

            [BurstCompile]
            public void Execute()
            {
                while (FarmResultsQueue.TryDequeue(out ulong farmed))
                {
                    TotalFarmed.Value += farmed;
                }
            }
        }

        [BurstCompile]
        public struct BananaFarmSystemChunkJobCombined : IJobChunk
        {
            public float DeltaTime;
            public ComponentTypeHandle<BananaAuthor.GroupedTimeToSpawn> ComponentHandle;
            public NativeQueue<ulong>.ParallelWriter FarmResultsQueue;

            [BurstCompile]
            public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
            {
                var timers = chunk.GetNativeArray(ref ComponentHandle);

                ulong localFarmed = 0;

                for (int i = 0; i < chunk.Count; i++)
                {
                    var current = timers[i];
                    current.CurrentTime += DeltaTime;
                    timers[i] = current;

                    if (current.CurrentTime >= current.TimeToSpawn)
                    {
                        current.CurrentTime = 0;
                        timers[i] = current;
                        localFarmed++;
                    }
                }

                if (localFarmed > 0)
                {
                    FarmResultsQueue.Enqueue(localFarmed);
                }
            }
        }
    }
}
