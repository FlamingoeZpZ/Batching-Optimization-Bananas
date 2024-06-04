using System.Collections.Generic;
using UnityEngine;

namespace Mono.GroupTimerBatch
{
    public class BananaFarmGroupManager : MonoBehaviour
    {
        private static readonly Dictionary<uint, BananaFarmGroupBatch> Batches = new();
        private static uint _id;
        
        //moved responsibility to here.
        [SerializeField] private float bananaTime;
        private float _currentTime;
        
        //Because we need a dictionary, or a list. we must choose to sacrifice memory or cpu.
        public static uint AddToBatch(BananaFarmGroupBatch val)
        {
            Batches.Add(_id, val);
            return _id++;
        }
        public static void RemoveFromBatch(uint id)
        {
            Batches.Remove(id);
        }
        
        private void Awake()
        {
            Batches.Clear();
        }
        
        private void OnDestroy(){
            Batches.Clear();
        }

        private void Update()
        {
            //Iterate through each object.
            _currentTime += Time.deltaTime;
            if (_currentTime >= bananaTime)
            {
                _currentTime = 0;
                foreach (var farm in Batches.Values)
                    farm.SpawnBanana();
            }
        }
    }
}