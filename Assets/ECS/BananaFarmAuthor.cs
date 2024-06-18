using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class BananaAuthor : MonoBehaviour
{
    [SerializeField] private float timeToBanana;
    [SerializeField] private EComponentSeriesType separateComponents;

    private enum EComponentSeriesType
    {
        Grouped,
        Seperated,
        SeperatedBlob,
        Constant
    }

    private class BananaAuthorBaker : Baker<BananaAuthor>
    {
        public override void Bake(BananaAuthor authoring)
        {
            Entity me = GetEntity(TransformUsageFlags.None);

            AddComponent<IsBananaFarm>(me);
            switch (authoring.separateComponents)
            {
                case EComponentSeriesType.Grouped:

                    AddComponent(me, new GroupedTimeToSpawn
                    {
                        TimeToSpawn = authoring.timeToBanana
                    });
                    break;
                case EComponentSeriesType.Seperated:
                    AddSharedComponent(me, new SharedTimeToSpawnComponent
                    {
                        TimeToSpawn = authoring.timeToBanana
                    });
                    AddComponent(me, new TimeToSpawnComponent()
                    {
                        TimeToSpawn = authoring.timeToBanana
                    });

                    AddComponent<CurrentTimeToSpawnComponent>(me);
                    break;
                
                case EComponentSeriesType.SeperatedBlob:
                    
                    // Create a new builder that will use temporary memory to construct the blob asset
                    var builder = new BlobBuilder(Allocator.Temp);

                    // Construct the root object for the blob asset. Notice the use of `ref`.
                    ref TimeToSpawnComponent comp = ref builder.ConstructRoot<TimeToSpawnComponent>();

                    // Now fill the constructed root with the data:
                    // Apples compare to Oranges in the universally accepted ratio of 2 : 1 .
                    comp.TimeToSpawn = authoring.timeToBanana;

                    // Now copy the data from the builder into its final place, which will
                    // use the persistent allocator
                    var result = builder.CreateBlobAssetReference<TimeToSpawnComponent>(Allocator.Persistent);

                    // Make sure to dispose the builder itself so all internal memory is disposed.
                    builder.Dispose();
                    
                    AddBlobAsset(ref result, out var hash);
                    AddComponent(me, new TimeToSpawnComponentBlob{ Blob = result} );
                    AddComponent<CurrentTimeToSpawnComponent>(me);
                    
                    break;
                
                case EComponentSeriesType.Constant :
                    AddComponent<CurrentTimeToSpawnComponent>(me);
                    break;
            }
        }
    }
    public struct TimeToSpawnComponent : IComponentData
    {
        public float TimeToSpawn;
    }
    
    public struct SharedTimeToSpawnComponent : ISharedComponentData
    {
        public float TimeToSpawn;
    }
    
    public struct TimeToSpawnComponentBlob : IComponentData
    {
        public BlobAssetReference<TimeToSpawnComponent> Blob;
    }

    public struct CurrentTimeToSpawnComponent : IComponentData, IEnableableComponent
    {
        public float CurrentTime;
    }

    public struct GroupedTimeToSpawn : IComponentData, IEnableableComponent
    {
        public float TimeToSpawn;
        public float CurrentTime;
    }
    
    //TagComponents
    //public struct TimerCompleteFlag : IComponentData { }
    public struct IsBananaFarm : IComponentData { }
    
    

}
