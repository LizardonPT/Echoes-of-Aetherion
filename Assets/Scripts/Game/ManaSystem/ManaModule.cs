using System;
using NaughtyAttributes;
using UnityEngine;

namespace EchoesOfEtherion.ManaSystem
{
    public class ManaModule : MonoBehaviour
    {
        [field: SerializeField] public float MaxMana { get; private set; }
        [field: SerializeField] public float CurrentMana { get; private set; }
        [field: SerializeField] public float ManaPerSecond { get; private set; }

        public event Action<float> ManaChanged;

        private void Awake()
        {
            CurrentMana = MaxMana;
        }

        private void Update()
        {
            RestoreManaOverTime(ManaPerSecond);
        }

        public void ConsumeMana(float manaCost)
        {
            CurrentMana -= manaCost;
            if (CurrentMana < 0)
            {
                CurrentMana = 0;
            }

            ManaChanged?.Invoke(CurrentMana);
        }

        public void RestoreMana(float manaRestored)
        {
            CurrentMana += manaRestored;
            if (CurrentMana > MaxMana)
            {
                CurrentMana = MaxMana;
            }

            ManaChanged?.Invoke(CurrentMana);
        }

        public void RestoreManaOverTime(float manaPerSecond)
        {
            if (CurrentMana < MaxMana)
            {
                CurrentMana += manaPerSecond * Time.deltaTime;
            }
            else
            {
                CurrentMana = MaxMana;
            }
            ManaChanged?.Invoke(CurrentMana);
        }



#if UNITY_EDITOR
        [Button("Give mana")]
        public void GiveManaEditor()
        {
            RestoreMana(10f);
        }

        [Button("Drain mana")]
        public void DrainManaEditor()
        {
            ConsumeMana(10f);
        }
#endif
    }
}
