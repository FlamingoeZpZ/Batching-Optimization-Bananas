using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;

namespace ECS
{
    [BurstCompile]
    public partial struct BananaFarmSystemJobChunk : ISystem
    {

        private EntityQuery _query;
        private ComponentTypeHandle<BananaAuthor.TimeToSpawnComponent> _timeToSpawnComponent;
        private ComponentTypeHandle<BananaAuthor.CurrentTimeToSpawnComponent> _currentTimeToSpawnComponent;
        private EntityTypeHandle _entityHandle;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _query = SystemAPI.QueryBuilder().WithAll<BananaAuthor.TimeToSpawnComponent>().WithAllRW<BananaAuthor.CurrentTimeToSpawnComponent>().Build();

            _timeToSpawnComponent = state.GetComponentTypeHandle<BananaAuthor.TimeToSpawnComponent>(true);
            _currentTimeToSpawnComponent = state.GetComponentTypeHandle<BananaAuthor.CurrentTimeToSpawnComponent>(false);
            _entityHandle = state.GetEntityTypeHandle();
            
            state.RequireForUpdate(_query);
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<SingletonAuthor.ChunkFarmer>();

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            return;
            EntityCommandBuffer ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
            
            _timeToSpawnComponent.Update(ref state);
            _currentTimeToSpawnComponent.Update(ref state);

            state.Dependency = new BananaFarmSystemChunkJob
            {
                ECB = ecb.AsParallelWriter(),
                TimeToSpawnComponent = _timeToSpawnComponent,
                CurrentTimeToSpawnComponent = _currentTimeToSpawnComponent,
                DeltaTime = SystemAPI.Time.DeltaTime,
                EntityHandle = _entityHandle
            }.ScheduleParallel(_query, state.Dependency);
        }
    }
    [BurstCompile]
    public struct BananaFarmSystemChunkJob : IJobChunk
    {
        
        public float DeltaTime;
        public EntityCommandBuffer.ParallelWriter ECB;
        [ReadOnly] public EntityTypeHandle EntityHandle;
        [ReadOnly] public ComponentTypeHandle<BananaAuthor.TimeToSpawnComponent> TimeToSpawnComponent;
        public ComponentTypeHandle<BananaAuthor.CurrentTimeToSpawnComponent> CurrentTimeToSpawnComponent;
        
        [BurstCompile]
        public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
        {
            NativeArray<BananaAuthor.TimeToSpawnComponent> timeToSpawn = chunk.GetNativeArray(ref TimeToSpawnComponent);
            NativeArray<BananaAuthor.CurrentTimeToSpawnComponent> timers = chunk.GetNativeArray(ref CurrentTimeToSpawnComponent);
            NativeArray<Entity> entities = chunk.GetNativeArray(EntityHandle);
            for (int i = 0; i < chunk.Count; i++)
            {
                BananaAuthor.CurrentTimeToSpawnComponent current = timers[i];
                current.CurrentTime += DeltaTime;
                timers[i] = current;

                if (current.CurrentTime >= timeToSpawn[i].TimeToSpawn)
                {
                    //ECB.AddComponent<BananaAuthor.TimerCompleteFlag>(unfilteredChunkIndex, entities[i]);
                }
            }
        }
    }
}