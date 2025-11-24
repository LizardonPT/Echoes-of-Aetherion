using UnityEngine;
using NaughtyAttributes;
using System;

namespace EchoesOfEtherion.CurrencySystem
{
    public class GoldModule : MonoBehaviour
    {
        [SerializeField] private int currentGold;

        public int CurrentGold
        {
            get => currentGold;
            private set
            {
                currentGold = value < 0 ? 0 : value;
                GoldChanged?.Invoke(currentGold);
            }
        }

        public event Action<int> GoldChanged;

        private void Awake()
        {
            CurrentGold = 0;
        }

        public void AddGold(int amount)
        {
            CurrentGold += amount;
        }

        public bool SpendGold(int amount)
        {
            if (amount > CurrentGold)
            {
                return false;
            }

            CurrentGold -= amount;
            return true;
        }

#if UNITY_EDITOR
        [Button("Give Gold")]
        public void GiveGoldEditor()
        {
            AddGold(50);
        }

        [Button("Take Gold")]
        public void TakeGoldEditor()
        {
            SpendGold(50);
        }
#endif
    }
}
