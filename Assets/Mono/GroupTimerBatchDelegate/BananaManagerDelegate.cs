using System;
using UnityEngine;

namespace Mono.GroupTimerBatchDelegate
{
    public class BananaManagerDelegate : MonoBehaviour
    {
        public static event Action OnTimerComplete;
        [SerializeField] private float bananaTime;
        private float _currentTime;

        private void Awake()
        {
            OnTimerComplete = null;
        }

        private void OnDestroy()
        {
            OnTimerComplete = null;
        }

        private void Update()
        {
            _currentTime += Time.deltaTime;
            if (_currentTime >= bananaTime)
            {
                OnTimerComplete?.Invoke();
                _currentTime = 0;
            }
        }

        public static void Register(BananaFarmGroupDelegate farmDelegate)
        {
            OnTimerComplete += farmDelegate.SpawnBanana;
        }

        public static void Unregister(BananaFarmGroupDelegate farmDelegate)
        {
            OnTimerComplete -= farmDelegate.SpawnBanana;
        }
    }
}