using UnityEngine;
using UnityEngine.InputSystem;
using EchoesOfEtherion.StateMachine;
using EchoesOfEtherion.Player.States;
using EchoesOfEtherion.CameraUtils;
using EchoesOfEtherion.Game;
using EchoesOfEtherion.Menu;
using EchoesOfEtherion.ScriptableObjects.Utils;
using EchoesOfEtherion.ScriptableObjects.Channels;
using System;

namespace EchoesOfEtherion.Player.Components
{
    [RequireComponent(typeof(PlayerMovement), typeof(PlayerAnimations))]
    [RequireComponent(typeof(PlayerInteractor))]
    public class PlayerController : TickRegistor
    {
        [field: SerializeField]
        public InputReader PlayerInput { get; private set; }
        [SerializeField] private CameraChannel cameraChannel;
        [field: Space]

        public PlayerAnimations Animator { get; private set; }
        public PlayerMovement Movement { get; private set; }
        public PlayerInteractor Interactor { get; private set; }

        public FiniteStateMachine<PlayerController> StateMachine { get; private set; }

        public Vector2 LookDirection
        {
            get
            {
                Vector2 pointerPos = Pointer.current != null
                    ? cameraChannel.GameCamera.ScreenToWorldPoint(Pointer.current.position.ReadValue())
                    : Vector2.zero;

                return (pointerPos != Vector2.zero ?
                    pointerPos - (Vector2)transform.position : Vector2.zero).normalized;
            }
        }

        private void Awake()
        {
            OnValidate();

            SetupStateMachine();
        }

        protected override void Start()
        {
            base.Start();
            
            try
            {
                cameraChannel.CameraFollow.SetTarget(transform);
            }
            catch (Exception ex)
            {
                Debug.Log($"[PlayerController] {ex.Message}");
            }
        }

        public override void Tick() => StateMachine?.Update();
        public override void FixedTick() => StateMachine?.FixedUpdate();

        private void SetupStateMachine()
        {
            StateMachine = new FiniteStateMachine<PlayerController>(this);

            StateMachine.AddState<PlayerIdleState>(new PlayerIdleState());
            StateMachine.AddState<PlayerMovingState>(new PlayerMovingState());
            StateMachine.ChangeState<PlayerIdleState>();
        }

        private void OnValidate()
        {
            Animator ??= GetComponent<PlayerAnimations>();
            Movement ??= GetComponent<PlayerMovement>();
            Interactor ??= GetComponent<PlayerInteractor>();
        }
    }
}