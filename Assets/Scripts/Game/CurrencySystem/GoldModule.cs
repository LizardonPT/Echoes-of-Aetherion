using UnityEngine;
using NaughtyAttributes;

namespace EchoesOfEtherion.CurrencySystem
{
    public class GoldModule : MonoBehaviour
    {
        [field: SerializeField] public int CurrentGold { get; private set; }

        public event System.Action<int> GoldChanged;

        private void Awake()
        {
            // Initialize gold if needed
            CurrentGold = 0;
        }

        public void AddGold(int amount)
        {
            CurrentGold += amount;
            GoldChanged?.Invoke(CurrentGold);
        }

        public bool SpendGold(int amount)
        {
            if (amount > CurrentGold)
            {
                return false; // Not enough gold
            }

            CurrentGold -= amount;
            GoldChanged?.Invoke(CurrentGold);
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
