using EchoesOfEtherion.Game;
using UnityEngine;
using UnityEngine.UI;

namespace EchoesOfEtherion.Menu
{
    public class HUDMenu : MonoBehaviour
    {
        [SerializeField] private Button pauseButton;


        private void Start()
        {
            pauseButton.onClick.AddListener(OnPauseClicked);
        }

        private void OnPauseClicked()
        {
            GameMaster.Instance.TogglePauseGame();
        }
    }
}
