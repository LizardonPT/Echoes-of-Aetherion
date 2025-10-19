using EchoesOfEtherion.Game.States;
using EchoesOfEtherion.Menu;
using EchoesOfEtherion.Player.Components;
using EchoesOfEtherion.ScriptableObjects.Utils;
using EchoesOfEtherion.StateMachine;
using UnityEngine;

namespace EchoesOfEtherion.Game
{
    [RequireComponent(typeof(TickController))]
    public class GameMaster : MonoBehaviour
    {
        [field: Header("ScriptableObjects")]
        [field: SerializeField] public InputReader InputReader { get; private set; }
        [field: Space]
        [field: Header("References")]
        [field: SerializeField] public MenuController MenuController { get; private set; }

        public FiniteStateMachine<GameMaster> StateMachine { get; private set; }
        private TickController tickController;

        private bool gamePaused = false;

        private void Awake()
        {
            InputReader ??= GetComponent<InputReader>();
            tickController ??= GetComponent<TickController>();
            SetupStateMachine();
        }

        private void Start()
        {
            MenuController ??= FindAnyObjectByType<MenuController>();
        }

        public void PauseGame()
        {
            if (!gamePaused)
            {
                tickController.SetPaused(true);
                Time.timeScale = 0f;
                gamePaused = true;
            }
        }

        public void ResumeGame()
        {
            if (gamePaused)
            {
                tickController.SetPaused(false);
                Time.timeScale = 1f;
                gamePaused = false;
            }
        }

        private void Update()
        {
            StateMachine?.Update();
        }

        private void SetupStateMachine()
        {
            StateMachine = new FiniteStateMachine<GameMaster>(this);

            StateMachine.AddState<GameplayState>(new GameplayState());
            StateMachine.AddState<GamePauseState>(new GamePauseState());

            StateMachine.ChangeState<GameplayState>();
        }
    }
}
