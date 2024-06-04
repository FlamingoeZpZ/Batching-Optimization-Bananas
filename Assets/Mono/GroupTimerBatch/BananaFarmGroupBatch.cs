using UnityEngine;

namespace Mono.GroupTimerBatch
{
    public class BananaFarmGroupBatch : MonoBehaviour
    {
        private uint _id;
        
        
        public void OnEnable()
        {
            _id = BananaFarmGroupManager.AddToBatch(this);
        }

        private void OnDisable()
        {
            BananaFarmGroupManager.RemoveFromBatch(_id);
        }
        
        public void SpawnBanana()
        {
            UIManager.AddBanana();
        }
    }
}