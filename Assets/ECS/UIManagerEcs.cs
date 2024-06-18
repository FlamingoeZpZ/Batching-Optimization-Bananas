using System;
using Unity.Entities;
using UnityEngine;
using TMPro;
using Unity.Collections;
using UnityEngine.UI;

namespace Mono
{
    public class UIManagerEcs : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI bananasSpawnedText;
        [SerializeField] private TextMeshProUGUI numFarmsText;
        [SerializeField] private TextMeshProUGUI sliderText;
        [SerializeField] private Slider slider;
        [SerializeField] private int maxFarms;
        [SerializeField] private GameObject prefab;

        private static UIManagerEcs _manager;
        private int _numFarms;

        private EntityManager _entityManager;

        private void Awake()
        {
            if (_manager && _manager != this)
            {
                Destroy(gameObject);
                return;
            }

            _manager = this;
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            slider.onValueChanged.AddListener(v => sliderText.text = v.ToString("N0"));

            numFarmsText.text = _numFarms.ToString();
            
        }


        public void AddRemoveFarms()
        {
            int farms = Mathf.Clamp(_numFarms + (int)slider.value, 0, maxFarms);
            UpdateFarmCount(farms);
        }

        public void AddRemoveFarms(int amount)
        {
            int farms = Mathf.Clamp(_numFarms + amount, 0, maxFarms);
            UpdateFarmCount(farms);
        }

        private void UpdateFarmCount(int farms)
        {
            int difference = farms - _numFarms;
            _numFarms += difference;

            if (difference < 0)
            {
                RemoveFarms(difference);
            }
            else if (difference > 0)
            {
                AddFarms(difference);
            }

            numFarmsText.text = _numFarms.ToString();
        }

        private void AddFarms(int count)
        {
            _entityManager.SetComponentData(_entityManager.CreateEntityQuery(typeof(SingletonAuthor.NumBananasFarmed)).ToEntityArray(Allocator.Temp)[0], new SingletonAuthor.BananaFarmNumToSpawn { NumToSpawn  = count });
        }

        private void RemoveFarms(int count)
        {
            _entityManager.SetComponentData(_entityManager.CreateEntityQuery(typeof(SingletonAuthor.NumBananasFarmed)).ToEntityArray(Allocator.Temp)[0], new SingletonAuthor.BananaFarmNumToSpawn { NumToSpawn  = count });

        }

        private void Update()
        {
            var x  = _entityManager.CreateEntityQuery(typeof(SingletonAuthor.NumBananasFarmed))
                .ToComponentDataArray<SingletonAuthor.NumBananasFarmed>(Allocator.Temp);
            if (x.Length == 0) return;
            bananasSpawnedText.text = (x[0].Value).ToString();
        }
    }
}