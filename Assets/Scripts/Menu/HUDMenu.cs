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

        private HealthModule playerHealth;
        private ManaModule playerMana;

        private void Start()
        {
            SceneLoader.Instance.SceneLoaded += OnSceneLoaded;
            pauseButton.onClick.AddListener(OnPauseClicked);

            PlayerController player = FindAnyObjectByType<PlayerController>();

            if (player != null)
            {
                playerHealth = player.GetComponent<HealthModule>();
                playerHealth.HealthChanged += UpdateHealthBar;
                UpdateHealthBar(playerHealth.CurrentHealth);
                playerMana = player.GetComponent<ManaModule>();
                playerMana.ManaChanged += UpdateManaBar;
                UpdateManaBar(playerMana.CurrentMana);
            }
        }

        private void OnSceneLoaded(string sceneName)
        {
            PlayerController player = FindAnyObjectByType<PlayerController>();
            if (player != null)
            {
                playerHealth = player.GetComponent<HealthModule>();
                playerHealth.HealthChanged += UpdateHealthBar;
                UpdateHealthBar(playerHealth.CurrentHealth);
                playerMana = player.GetComponent<ManaModule>();
                playerMana.ManaChanged += UpdateManaBar;
                UpdateManaBar(playerMana.CurrentMana);
            }
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
            healthBar.maxValue = playerHealth.MaxHealth;
            float v = Mathf.Clamp(currentHealth, 0, playerHealth.MaxHealth);
            healthBar.value = v;
            healthText.text = $"{v}/{playerHealth.MaxHealth}";
        }

        private void UpdateManaBar(float currentMana)
        {
            manaBar.maxValue = playerMana.MaxMana;
            int currentManaRounded = Mathf.RoundToInt(currentMana);
            int v = Mathf.Clamp(currentManaRounded, 0, (int)playerMana.MaxMana);
            manaBar.value = v;
            manaText.text = $"{v}/{playerMana.MaxMana}";
        }

        private void OnPauseClicked()
        {
            GameMaster.Instance.TogglePauseGame();
        }
    }
}
