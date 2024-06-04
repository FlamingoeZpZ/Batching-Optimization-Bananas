using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mono.IndividualTimerBatch
{
    public class BananaManagerBatchTimer : MonoBehaviour
    {
        private static readonly Dictionary<uint, BananaFarmBatchTimer> Batches = new();
        private static uint _id;
        
        //Because we need a dictionary, or a list. we must choose to sacrifice memory or cpu.
        public static uint AddToBatch(BananaFarmBatchTimer val)
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
            float deltaTime = Time.deltaTime;
            foreach (var farm in Batches.Values) farm.Tick(deltaTime);
        }

        
    }
}