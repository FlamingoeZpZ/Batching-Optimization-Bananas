using UnityEngine;

namespace Mono.GroupTimerBatchDelegate
{
    public class BananaFarmGroupDelegate : MonoBehaviour
    {
        private void OnEnable()
        {
            BananaManagerDelegate.Register(this);
        }

        private void OnDisable()
        {
            BananaManagerDelegate.Unregister(this);
        }

        public void SpawnBanana()
        {
            UIManager.AddBanana();
        }
    }
}