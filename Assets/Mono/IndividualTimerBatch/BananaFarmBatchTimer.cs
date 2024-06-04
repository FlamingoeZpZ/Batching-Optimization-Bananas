using UnityEngine;

namespace Mono.IndividualTimerBatch
{
    public class BananaFarmBatchTimer : MonoBehaviour
    {
        [SerializeField] private float bananaTime;
        private float _currentTime;
        private uint _id;

        public void OnEnable()
        {
            _id = BananaManagerBatchTimer.AddToBatch(this);
        }

        private void OnDisable()
        {
            BananaManagerBatchTimer.RemoveFromBatch(_id);
        }
        

        public void Tick(float duration)
        {
            _currentTime += duration;
            if (_currentTime >= bananaTime)
                SpawnBanana();
        }

        public void SpawnBanana()
        {
            UIManager.AddBanana();
            _currentTime = 0;
        }
    }
}