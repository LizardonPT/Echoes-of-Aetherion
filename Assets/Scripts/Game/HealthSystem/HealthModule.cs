using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace EchoesOfEtherion.HealthSystem
{
    public class HealthModule : MonoBehaviour
    {
        [field: SerializeField] public float MaxHealth { get; private set; }
        [field: SerializeField] public float CurrentHealth { get; private set; }

        [SerializeField] private UnityEvent onDamaged;
        [SerializeField] private UnityEvent onHealed;
        [SerializeField] private UnityEvent onDied;

        public event Action<DamageInfo> Damaged;
        public event Action<float> Healed;
        public event Action<float> HealthChanged;
        public event Action<HealthModule> Died;

        public bool IsDead { get; private set; } = false;

        private void Awake()
        {
            IsDead = false;
            CurrentHealth = MaxHealth;
        }

        public void Damage(GameObject damager, float damageAmount, float knockback, float stunTime = 0)
        {
            if (IsDead) return;
            CurrentHealth -= damageAmount;
            if (CurrentHealth <= 0)
            {
                CurrentHealth = 0;

                Damaged?.Invoke(
                    new DamageInfo(damager, this, damageAmount, damager.transform.position, knockback, stunTime)
                    );
                onDamaged?.Invoke();

                HealthChanged?.Invoke(CurrentHealth);
                Died?.Invoke(this);
                onDied?.Invoke();
                IsDead = true;
                return;
            }

            Damaged?.Invoke(
                new DamageInfo(damager, this, damageAmount, damager.transform.position, knockback, stunTime)
                );
            onDamaged?.Invoke();
            HealthChanged?.Invoke(CurrentHealth);
        }

        public void Heal(float healAmount)
        {
            if (IsDead) return;

            CurrentHealth += healAmount;
            if (CurrentHealth >= MaxHealth)
            {
                CurrentHealth = MaxHealth;
            }

            Healed?.Invoke(healAmount);
            onHealed?.Invoke();
            HealthChanged?.Invoke(CurrentHealth);
        }

#if UNITY_EDITOR
        [Button("Damage")]
        public void DamageEditor()
        {
            Damage(gameObject, 20, 0, 0);
        }
        
        [Button("Heal")]
        public void HealEditor()
        {
            Heal(20);
        }
#endif
    }
}
