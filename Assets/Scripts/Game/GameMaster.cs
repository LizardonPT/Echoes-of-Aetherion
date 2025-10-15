using EchoesOfAetherion.Game.States;
using EchoesOfAetherion.Inputs;
using EchoesOfAetherion.Player.Components;
using EchoesOfAetherion.StateMachine;
using UnityEngine;

namespace EchoesOfAetherion.Game
{
    [RequireComponent(typeof(InputReader))]
    public class GameMaster : MonoBehaviour
    {
        public InputReader InputReader { get; private set; }
        public FiniteStateMachine<GameMaster> StateMachine { get; private set; }

        [field: SerializeField] public MenuController MenuController { get; private set; }
        [field: SerializeField] public PlayerController Player { get; private set; }

        private void Awake()
        {
            SetupStateMachine();
            OnValidate();
        }

        private void Start()
        {
            Player ??= FindAnyObjectByType<PlayerController>();
            MenuController ??= FindAnyObjectByType<MenuController>();
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

        private void OnValidate()
        {
            InputReader ??= GetComponent<InputReader>();
        }
    }
}
