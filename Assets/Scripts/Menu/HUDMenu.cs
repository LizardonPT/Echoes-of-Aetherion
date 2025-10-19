using EchoesOfEtherion.Game;
using UnityEngine;
using UnityEngine.UI;

namespace EchoesOfEtherion.Menu
{
    public class HUDMenu : MonoBehaviour
    {
        [SerializeField] private Button pauseButton;

        private GameMaster gameMaster;
        private MenuController menuController;

        private void Start()
        {
            pauseButton.onClick.AddListener(OnPauseClicked);
            gameMaster ??= FindAnyObjectByType<GameMaster>();

            menuController ??= FindAnyObjectByType<MenuController>();
        }

        private void OnPauseClicked()
        {
            gameMaster?.PauseGame();
            menuController.ShowPauseMenu();
        }
    }
}
