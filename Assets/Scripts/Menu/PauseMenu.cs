using UnityEngine;
using UnityEngine.UI;
using EchoesOfEtherion.ScriptableObjects.Channels;
using EchoesOfEtherion.Game;

namespace EchoesOfEtherion.Menu
{
    public class PauseMenu : MonoBehaviour
    {
        [Header("Channel References")]
        [SerializeField] private SceneLoaderChannel sceneLoaderChannel;

        [Header("UI References")]
        [SerializeField] private GameObject pauseMenuPanel;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button quitButton;

        [Header("Components References")]
        [SerializeField] private GameMaster gameMaster;

        private void Awake()
        {
            // Setup button listeners
            resumeButton?.onClick.AddListener(OnResumeButtonClicked);
            mainMenuButton?.onClick.AddListener(OnMainMenuButtonClicked);
            quitButton?.onClick.AddListener(OnQuitButtonClicked);

            // Hide pause menu initially
            HidePauseMenu();
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
            gameMaster?.ResumeGame();
            HidePauseMenu();
        }

        private void OnMainMenuButtonClicked()
        {
            // Use the SceneLoader to return to main menu from current scene
            if (sceneLoaderChannel != null)
            {
                sceneLoaderChannel.RequestSwitchScene("MainMenu");
            }
            else
            {
                Debug.LogError("[PauseMenu] SceneLoaderChannel reference is null!");
            }
        }

        private void OnQuitButtonClicked()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}