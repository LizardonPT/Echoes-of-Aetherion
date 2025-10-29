using EchoesOfEtherion.Game;
using EchoesOfEtherion.Game.Scenes;
using EchoesOfEtherion.ScriptableObjects.Channels;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace EchoesOfEtherion.Menu
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private SceneLoaderChannel sceneLoaderChannel;
        [SerializeField] private Button startButton;
        [SerializeField] private Button quitButton;


        private void Start()
        {
            startButton.onClick.AddListener(StartGame);
            quitButton.onClick.AddListener(Quit);
        }

        private void StartGame()
        {
            sceneLoaderChannel.RequestSwitchScene("Prototype", "MainMenu");
        }

        private void Quit()
        {
            Application.Quit();
        }
    }
}
