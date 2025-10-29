using EchoesOfEtherion.Game;
using UnityEngine;
using UnityEngine.UI;

namespace EchoesOfEtherion.Menu
{
    public class HUDMenu : MonoBehaviour
    {
        [SerializeField] private Button pauseButton;

        private GameMaster gameMaster;
        private PauseMenu menuController;

        private void Start()
        {
            pauseButton.onClick.AddListener(OnPauseClicked);
            gameMaster ??= FindAnyObjectByType<GameMaster>();

            menuController ??= FindAnyObjectByType<PauseMenu>();
        }

        private void OnPauseClicked()
        {
            gameMaster?.PauseGame();
            menuController.ShowPauseMenu();
        }
    }
}
