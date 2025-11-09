using EchoesOfEtherion.Game;
using EchoesOfEtherion.Game.Scenes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace EchoesOfEtherion.Menu
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private Button startButton;
        [SerializeField] private Button quitButton;

        private void Start()
        {
            startButton.onClick.AddListener(StartGame);
            quitButton.onClick.AddListener(Quit);
        }

        private void StartGame()
        {
            SceneLoader.Instance.SwitchToScene("Prototype");
        }

        private void Quit()
        {
            SceneLoader.Instance.QuitGame();
        }
    }
}
