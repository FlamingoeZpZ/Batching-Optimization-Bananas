using UnityEngine;

namespace Mono.Solo
{
    public class BananaFarmSolo : MonoBehaviour
    {

        [SerializeField] private float bananaTime;
        private float _currentTime;

        private void Update()
        {
            _currentTime += Time.deltaTime;
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