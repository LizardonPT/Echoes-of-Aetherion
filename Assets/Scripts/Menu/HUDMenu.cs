using EchoesOfEtherion.Game;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace EchoesOfEtherion.Menu
{
    public class HUDMenu : MonoBehaviour
    {
        [SerializeField] private Button pauseButton;
        [SerializeField] private Slider healthBar;
        [SerializeField] private Slider manaBar;


        private void Start()
        {
            HealthSystem healthSystem = FindFirstObjectByType<HealthSystem>();
            healthBar.maxValue = healthSystem.MaxHealth;
            healthSystem.OnHealthChanged += UpdateHealthBar;
            UpdateHealthBar(healthSystem.CurrentHealth);
            pauseButton.onClick.AddListener(OnPauseClicked);
        }
        
        private void UpdateHealthBar(float currentHealth)
        {
            healthBar.value = currentHealth;
            
            if (currentHealth <= 0)
            {
                healthBar.value = 0;
            }
        }

        private void OnPauseClicked()
        {
            GameMaster.Instance.TogglePauseGame();
        }
    }
}
