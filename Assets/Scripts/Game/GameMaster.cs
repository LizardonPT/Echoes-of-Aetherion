using EchoesOfAetherion.Game.States;
using EchoesOfAetherion.Inputs;
using EchoesOfAetherion.Player.Components;
using EchoesOfAetherion.StateMachine;
using UnityEngine;

namespace EchoesOfAetherion.Game
{
    [RequireComponent(typeof(InputReader), typeof(TickController))]
    public class GameMaster : MonoBehaviour
    {
        [field: SerializeField] public MenuController MenuController { get; private set; }
        [field: SerializeField] public PlayerController Player { get; private set; }

        public InputReader InputReader { get; private set; }
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
            Player ??= FindAnyObjectByType<PlayerController>();
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
