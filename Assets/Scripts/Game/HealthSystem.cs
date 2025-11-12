using System;
using NaughtyAttributes;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [field: SerializeField] public float MaxHealth { get; private set; }
    public float CurrentHealth { get; private set; }

    public event Action <float> Damaged;
    public event Action<float> Healed;
    public event Action Died;
    
    private void Awake()
    {
        CurrentHealth = MaxHealth;
    }

    public void Damage(float damageAmount)
    {
        CurrentHealth -= damageAmount;
        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            Died?.Invoke();
            return;
        }

        Damaged?.Invoke(damageAmount);
    }

    public void Heal(float healAmount)
    {
        CurrentHealth += healAmount;
        if (CurrentHealth >= MaxHealth)
        {
            CurrentHealth = MaxHealth;
            return;
        }

        Healed?.Invoke(healAmount);
    }
}
