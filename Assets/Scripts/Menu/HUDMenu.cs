using System;
using EchoesOfEtherion.Game;
using EchoesOfEtherion.Game.Scenes;
using EchoesOfEtherion.HealthSystem;
using EchoesOfEtherion.ManaSystem;
using EchoesOfEtherion.Player.Components;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

namespace EchoesOfEtherion.Menu
{
    public class HUDMenu : MonoBehaviour
    {
        [SerializeField] private Button pauseButton;
        [SerializeField] private Slider healthBar;
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private Slider manaBar;
        [SerializeField] private TextMeshProUGUI manaText;

        [SerializeField] private HealthModule playerHealth;
        [SerializeField] private ManaModule playerMana;

        private void Start()
        {
            pauseButton.onClick.AddListener(OnPauseClicked);

            playerHealth ??= GetComponentInParent<HealthModule>();
            playerMana ??= GetComponentInParent<ManaModule>();

            UpdateHealthBar(playerHealth.CurrentHealth);
            UpdateManaBar(playerMana.CurrentMana);
        }

        private void OnEnable()
        {
            if (playerHealth != null)
                playerHealth.HealthChanged += UpdateHealthBar;

            if (playerMana != null)
                playerMana.ManaChanged += UpdateManaBar;
        }

        private void OnDisable()
        {
            if (playerHealth != null)
                playerHealth.HealthChanged -= UpdateHealthBar;

            if (playerMana != null)
                playerMana.ManaChanged -= UpdateManaBar;
        }

        private void UpdateHealthBar(float currentHealth)
        {
            float p;
            if (currentHealth != 0 && playerHealth.MaxHealth != 0)
                p = currentHealth / playerHealth.MaxHealth;
            else p = 0;
            healthBar.value = p;
            healthText.text = $"{Mathf.FloorToInt(currentHealth)}/{playerHealth.MaxHealth}";
        }

        private void UpdateManaBar(float currentMana)
        {
            float p;
            if (currentMana != 0 && playerMana.MaxMana != 0)
                p = currentMana / playerMana.MaxMana;
            else p = 0;
            manaBar.value = p;
            manaText.text = $"{Mathf.FloorToInt(currentMana)}/{playerMana.MaxMana}";
        }

        private void OnPauseClicked()
        {
            GameMaster.Instance.TogglePauseGame();
        }
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            playerHealth ??= GetComponentInParent<HealthModule>();
            playerMana ??= GetComponentInParent<ManaModule>();
        }
#endif
    }
}
