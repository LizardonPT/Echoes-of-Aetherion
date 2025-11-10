using UnityEngine;
using UnityEngine.UI;
using EchoesOfEtherion.Game;
using EchoesOfEtherion.Game.Scenes;

namespace EchoesOfEtherion.Menu
{
    public class PauseMenu : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject pauseMenuPanel;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button quitButton;

        private void Awake()
        {
            // Setup button listeners
            resumeButton?.onClick.AddListener(OnResumeButtonClicked);
            mainMenuButton?.onClick.AddListener(OnMainMenuButtonClicked);
            quitButton?.onClick.AddListener(OnQuitButtonClicked);

            // Hide pause menu initially
            HidePauseMenu();
        }

        private void OnEnable()
        {
            GameMaster.Instance.GamePaused += ShowPauseMenu;
            GameMaster.Instance.GameResumed += HidePauseMenu;
        }

        private void OnDisable()
        {
            if (GameMaster.Instance == null) return;
            GameMaster.Instance.GamePaused -= ShowPauseMenu;
            GameMaster.Instance.GameResumed -= HidePauseMenu;
        }

        private void OnDestroy()
        {
            // Clean up listeners
            resumeButton?.onClick.RemoveListener(OnResumeButtonClicked);
            mainMenuButton?.onClick.RemoveListener(OnMainMenuButtonClicked);
            quitButton?.onClick.RemoveListener(OnQuitButtonClicked);
        }

        public void ShowPauseMenu()
        {
            pauseMenuPanel?.SetActive(true);
        }

        public void HidePauseMenu()
        {
            pauseMenuPanel?.SetActive(false);
        }

        private void OnResumeButtonClicked()
        {
            GameMaster.Instance?.RequestResume();
            HidePauseMenu();
        }

        private void OnMainMenuButtonClicked()
        {
            SceneLoader.Instance.SwitchToScene("MainMenu");
        }

        private void OnQuitButtonClicked()
        {
            SceneLoader.Instance.QuitGame();
        }
    }
}