using UnityEngine;
using UnityEngine.InputSystem;
using EchoesOfAetherion.StateMachine;
using EchoesOfAetherion.Player.States;
using EchoesOfAetherion.Player.Data;
using EchoesOfAetherion.Input;
using EchoesOfAetherion.CameraUtils;
namespace EchoesOfAetherion.Player.Components
{
    [RequireComponent(typeof(InputReader), typeof(PlayerMovement), typeof(PlayerAnimations))]
    public class PlayerController : MonoBehaviour
    {
        public PlayerData Data { get; private set; }
        public PlayerAnimations Animator { get; private set; }
        public PlayerMovement Movement { get; private set; }
        public FiniteStateMachine<PlayerController> StateMachine { get; private set; }

        private InputReader inputReader;
        private CameraFollow cameraFollow;

        private void Awake()
        {
            Data = new PlayerData();
            Animator = GetComponent<PlayerAnimations>();
            Movement = GetComponent<PlayerMovement>();
            StateMachine = new FiniteStateMachine<PlayerController>(this);
            inputReader = GetComponent<InputReader>();
            cameraFollow = Camera.main.GetComponent<CameraFollow>();

            SetupStateMachine();
        }

        private void Start()
        {
            cameraFollow.SetTarget(transform);
        }

        private void Update()
        {
            Data.MovementInput = inputReader.MovementInput;
            Vector2 pointerPos = Pointer.current != null
                ? Camera.main.ScreenToWorldPoint(Pointer.current.position.ReadValue())
                : (Vector2)transform.position;

            Data.LookDirection = (pointerPos - (Vector2)transform.position).normalized;

            StateMachine?.Update();
        }

        private void FixedUpdate()
        {
            StateMachine?.FixedUpdate();
        }

        private void SetupStateMachine()
        {
            StateMachine.AddState<PlayerIdleState>(new PlayerIdleState());
            StateMachine.AddState<PlayerMovingState>(new PlayerMovingState());
            StateMachine.ChangeState<PlayerIdleState>();
        }
    }
}