using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SingletonAuthor : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private EFarmingType type;
    enum EFarmingType
    {
        Idiomatic,
        Job,
        Chunk
    }

    private class SingletonAuthorBaker : Baker<SingletonAuthor>
    {
        public override void Bake(SingletonAuthor authoring)
        {
            var e = GetEntity(authoring.gameObject, TransformUsageFlags.None);

            AddComponent(e, new NumBananasFarmed()
            {
                Value = 0
            });
            AddComponent(e, new BananaFarmNumToSpawn()
            {
                NumToSpawn = 0
            });
            AddComponent(e, new BananaFarmPrefab()
            {
                Entity = GetEntity(authoring.prefab, TransformUsageFlags.None)
            });

            switch (authoring.type)
            {
         
                case EFarmingType.Idiomatic:
                    AddComponent<IdiomaticFarmer>(e);
                    break;
                case EFarmingType.Job:
                    AddComponent<JobFarmer>(e);
                    break;
                case EFarmingType.Chunk:
                    AddComponent<ChunkFarmer>(e);
                    break;
            }
        }
    }
    
    public struct NumBananasFarmed : IComponentData
    {
        public int Value;
    }

    public struct BananaFarmPrefab : IComponentData
    {
        public Entity Entity;
    }
    
    public struct BananaFarmNumToSpawn : IComponentData
    {
        public int NumToSpawn;
    }

    public struct IdiomaticFarmer : IComponentData { }
    public struct JobFarmer : IComponentData { }
    public struct ChunkFarmer : IComponentData { }
    
}
