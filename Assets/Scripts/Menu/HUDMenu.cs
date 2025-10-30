using EchoesOfEtherion.Game;
using UnityEngine;
using UnityEngine.UI;

namespace EchoesOfEtherion.Menu
{
    public class HUDMenu : MonoBehaviour
    {
        [SerializeField] private Button pauseButton;

        [SerializeField]
        private GameMaster gameMaster;
        [SerializeField]
        private PauseMenu pauseMenu;

        private void Start()
        {
            pauseButton.onClick.AddListener(OnPauseClicked);
        }

        private void OnPauseClicked()
        {
            gameMaster.PauseGame();
            pauseMenu.ShowPauseMenu();
        }
    }
}
