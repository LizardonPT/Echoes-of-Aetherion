using UnityEngine;
using UnityEngine.InputSystem;
using EchoesOfAetherion.StateMachine;
using EchoesOfAetherion.Player.States;
using EchoesOfAetherion.CameraUtils;
using EchoesOfAetherion.Inputs;

namespace EchoesOfAetherion.Player.Components
{
    [RequireComponent(typeof(PlayerMovement), typeof(PlayerAnimations), typeof(InputReader))]
    public class PlayerController : MonoBehaviour
    {
        [field: SerializeField] public MenuController MenuController { get; private set; }

        public PlayerAnimations Animator { get; private set; }
        public PlayerMovement Movement { get; private set; }
        public InputReader PlayerInput { get; private set; }

        public FiniteStateMachine<PlayerController> StateMachine { get; private set; }
        private CameraFollow cameraFollow;

        public Vector2 LookDirection
        {
            get
            {
                Vector2 pointerPos = Pointer.current != null
                    ? Camera.main.ScreenToWorldPoint(Pointer.current.position.ReadValue())
                    : Vector2.zero;

                return (pointerPos != Vector2.zero ?
                    pointerPos - (Vector2)transform.position : Vector2.zero).normalized;
            }
        }

        private bool canTick = false;

        private void Awake()
        {
            OnValidate();

            SetupStateMachine();
        }

        private void Start()
        {
            //! This gotta be a better way... (Code smell?)
            cameraFollow = Camera.main.GetComponent<CameraFollow>();

            cameraFollow?.SetTarget(transform);
        }

        private void Update()
        {
            if (canTick)
                StateMachine?.Update();
        }

        private void FixedUpdate()
        {
            if (canTick)
                StateMachine?.FixedUpdate();
        }

        private void SetupStateMachine()
        {
            StateMachine = new FiniteStateMachine<PlayerController>(this);

            StateMachine.AddState<PlayerIdleState>(new PlayerIdleState());
            StateMachine.AddState<PlayerMovingState>(new PlayerMovingState());
            StateMachine.ChangeState<PlayerIdleState>();
        }

        public void SetCanTick(bool value)
        {
            canTick = value;
        }

        private void OnValidate()
        {
            Animator ??= GetComponent<PlayerAnimations>();
            Movement ??= GetComponent<PlayerMovement>();
            PlayerInput ??= GetComponent<InputReader>();

            if (MenuController == null)
            {
                MenuController = FindAnyObjectByType<MenuController>();
                if (MenuController == null)
                    Debug.LogWarning("No Menu Controller in the scene"); ;
            }
        }
    }
}