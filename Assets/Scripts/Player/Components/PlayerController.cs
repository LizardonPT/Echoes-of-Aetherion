using UnityEngine;
using UnityEngine.InputSystem;
using EchoesOfAetherion.StateMachine;
using EchoesOfAetherion.Player.States;
using EchoesOfAetherion.CameraUtils;
namespace EchoesOfAetherion.Player.Components
{
    [RequireComponent(typeof(PlayerMovement), typeof(PlayerAnimations), typeof(PlayerInput))]
    public class PlayerController : MonoBehaviour
    {
        [Header("References")]
        [field: SerializeField] public PlayerAnimations Animator { get; private set; }
        [field: SerializeField] public PlayerMovement Movement { get; private set; }
        [field: SerializeField] public MenuController MenuController { get; private set; }
        [field: SerializeField] public PlayerInput PlayerInput { get; private set; }

        public FiniteStateMachine<PlayerController> StateMachine { get; private set; }
        private CameraFollow cameraFollow;
        public Vector2 LookDirection { get; private set; }

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
            StateMachine.AddState<PlayerPauseMenuState>(new PlayerPauseMenuState());
            StateMachine.ChangeState<PlayerIdleState>();
        }

        public void PauseGame()
        {
            StateMachine.ChangeState<PlayerPauseMenuState>();
        }

        private void OnValidate()
        {
            if (Animator == null) Animator = GetComponent<PlayerAnimations>();
            if (Movement == null) Movement = GetComponent<PlayerMovement>();
            if (PlayerInput == null) PlayerInput = GetComponent<PlayerInput>();

            if (MenuController == null)
            {
                MenuController = FindAnyObjectByType<MenuController>();
                if (MenuController == null)
                    Debug.LogWarning("No Menu Controller in the scene"); ;
            }
        }
    }
}