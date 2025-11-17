using EchoesOfEtherion.Game;
using EchoesOfEtherion.Game.Scenes;
using EchoesOfEtherion.Player.Components;
using TMPro;
using Unity.VisualScripting;
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

        private HealthSystem playerHealth;

        private void Start()
        {
            SceneLoader.Instance.SceneLoaded += OnSceneLoaded;
            pauseButton.onClick.AddListener(OnPauseClicked);

            PlayerController player = FindAnyObjectByType<PlayerController>();

            if (player != null)
            {
                playerHealth = player.GetComponent<HealthSystem>();
                playerHealth.HealthChanged += UpdateHealthBar;
                UpdateHealthBar(playerHealth.CurrentHealth);
            }
        }

        private void OnSceneLoaded(string sceneName)
        {
            PlayerController player = FindAnyObjectByType<PlayerController>();
            if (player != null)
            {
                playerHealth = player.GetComponent<HealthSystem>();
                playerHealth.HealthChanged += UpdateHealthBar;
                UpdateHealthBar(playerHealth.CurrentHealth);
            }
        }

        private void OnEnable()
        {
            if (playerHealth != null)
                playerHealth.HealthChanged += UpdateHealthBar;
        }

        private void OnDisable()
        {
            if (playerHealth != null)
                playerHealth.HealthChanged -= UpdateHealthBar;
        }

        private void UpdateHealthBar(float currentHealth)
        {
            healthBar.maxValue = playerHealth.MaxHealth;
            float v = Mathf.Clamp(currentHealth, 0, playerHealth.MaxHealth);
            healthBar.value = v;
            healthText.text = $"{v}/{playerHealth.MaxHealth}";
        }

        private void OnPauseClicked()
        {
            GameMaster.Instance.TogglePauseGame();
        }
    }
}
