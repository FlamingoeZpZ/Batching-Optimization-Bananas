using System;
using UnityEngine;
using TMPro;
using Unity.Burst;
using UnityEngine.UI;

namespace Mono
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI bananasSpawnedText;
        [SerializeField] private TextMeshProUGUI numFarmsText;
        [SerializeField] private TextMeshProUGUI sliderText;
        [SerializeField] private Slider slider;
        [SerializeField] private int maxFarms;
        [SerializeField] private GameObject prefab;
        private static UIManager _manager;
        private static ulong _bananasSpawned;
        private int _numFarms;

        private void Awake()
        {
            if (_manager && _manager != this)
            {
                Destroy(gameObject);
                return;
            }
            _manager = this;
            FarmOverlord.Objects.Clear();
            
            slider.onValueChanged.AddListener(v => sliderText.text = v.ToString("N0"));
            
            numFarmsText.text = _numFarms.ToString();
            bananasSpawnedText.text = _bananasSpawned.ToString();
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
            
            //Access the farm manager and tell it to discard or add that many farms
            if(difference < 0) FarmOverlord.RemoveItem(difference);
            else if (difference > 0) while(difference-- > 0) FarmOverlord.AddItem(Instantiate(prefab));
            
            numFarmsText.text = _numFarms.ToString();
            
            print("Farms: " + _numFarms +", " + difference);
        }
        public static void AddBanana()
        {
            ++_bananasSpawned;
        }

        private void Update()
        {
            bananasSpawnedText.text = _bananasSpawned.ToString();
        }
    }
}