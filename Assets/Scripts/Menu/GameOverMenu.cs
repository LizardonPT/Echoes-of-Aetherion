using EchoesOfEtherion.Game.Scenes;
using EchoesOfEtherion.HealthSystem;
using UnityEngine;
using UnityEngine.UI;

namespace EchoesOfEtherion.Meny
{
    public class GameOverMenu : MonoBehaviour
    {
        [SerializeField] private Button restartButton;
        [SerializeField] private Transform panel;

        [SerializeField] private HealthModule playerHealth;

        private void Start()
        {
            restartButton.onClick.AddListener(OnRestartButtonClick);
            panel.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            playerHealth.Died += OnDied;
        }

        private void OnDisable()
        {
            playerHealth.Died -= OnDied;
        }

        private void OnRestartButtonClick()
        {
            if (SceneLoader.Instance == null)
                return;

            string currentScene = SceneLoader.Instance.CurrentPrimaryScene;

            SceneLoader.Instance.LoadPrimaryScene(currentScene);
        }

        private void OnDied(HealthModule module)
        {
            panel.gameObject.SetActive(true);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            restartButton ??= GetComponentInChildren<Button>();
            panel ??= transform.GetChild(0);
            playerHealth ??= GetComponentInParent<HealthModule>();
        }
# endif
    }
}
