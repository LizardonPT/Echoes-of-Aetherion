using EchoesOfAetherion.Game;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private Button resumeButton;

    private GameMaster gameMaster;
    private MenuController menuController;

    private void Start()
    {
        resumeButton.onClick.AddListener(OnResumeClicked);
        gameMaster ??= FindAnyObjectByType<GameMaster>();
        menuController ??= FindAnyObjectByType<MenuController>();
    }

    private void OnResumeClicked()
    {
        gameMaster?.ResumeGame();
        menuController.HidePauseMenu();
    }
}
