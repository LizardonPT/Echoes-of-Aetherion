using UnityEngine;
using UnityEngine.InputSystem;
using EchoesOfAetherion.StateMachine;
using EchoesOfAetherion.Player.States;
using EchoesOfAetherion.CameraUtils;
namespace EchoesOfAetherion.Player.Components
{
    [RequireComponent(typeof(PlayerMovement), typeof(PlayerAnimations))]
    public class PlayerController : MonoBehaviour
    {
        [field: SerializeField] public PlayerAnimations Animator { get; private set; }
        [field: SerializeField] public PlayerMovement Movement { get; private set; }

        public FiniteStateMachine<PlayerController> StateMachine { get; private set; }

        //? Should the controller have the input actions?
        //? This vars are for states.
        [field: SerializeField] public InputActionReference MoveInputAction { get; private set; }
        public Vector2 LookDirection { get; private set; }

        private CameraFollow cameraFollow;

        private void Awake()
        {
            OnValidate();

            //! This gotta be a better way... (Code smell?)
            cameraFollow = Camera.main.GetComponent<CameraFollow>();

            SetupStateMachine();
        }

        private void Start()
        {
            cameraFollow?.SetTarget(transform);
        }

        private void Update()
        {
            Vector2 pointerPos = Pointer.current != null
                ? Camera.main.ScreenToWorldPoint(Pointer.current.position.ReadValue())
                : (Vector2)transform.position;

            LookDirection = (pointerPos - (Vector2)transform.position).normalized;

            StateMachine?.Update();
        }

        private void FixedUpdate()
        {
            StateMachine?.FixedUpdate();
        }

        private void SetupStateMachine()
        {
            StateMachine = new FiniteStateMachine<PlayerController>(this);

            StateMachine.AddState<PlayerIdleState>(new PlayerIdleState());
            StateMachine.AddState<PlayerMovingState>(new PlayerMovingState());
            StateMachine.ChangeState<PlayerIdleState>();
        }

        private void OnValidate()
        {
            if (Animator == null) Animator = GetComponent<PlayerAnimations>();
            if (Movement == null) Movement = GetComponent<PlayerMovement>();
        }
    }
}