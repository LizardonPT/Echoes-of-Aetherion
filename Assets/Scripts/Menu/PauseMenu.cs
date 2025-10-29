using EchoesOfEtherion.Game;
using UnityEngine;
using UnityEngine.UI;

namespace EchoesOfEtherion.Menu
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private GameObject pausedMenu;
        [SerializeField] private Button resumeButton;

        private GameMaster gameMaster;
        private void Start()
        {
            resumeButton.onClick.AddListener(OnResumeClicked);
            gameMaster ??= FindAnyObjectByType<GameMaster>();
        }

        private void OnResumeClicked()
        {
            gameMaster?.ResumeGame();
            HidePauseMenu();
        }

        public void ShowPauseMenu()
        {
            pausedMenu?.SetActive(true);
        }

        public void HidePauseMenu()
        {
            pausedMenu?.SetActive(false);
        }
    }
}
