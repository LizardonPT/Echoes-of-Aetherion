using UnityEngine;
using UnityEngine.InputSystem;
using EchoesOfEtherion.Game.StateMachine;
using EchoesOfEtherion.Player.States;
using EchoesOfEtherion.CameraUtils;
using EchoesOfEtherion.Game;
using EchoesOfEtherion.Game.Utils;
using System;
using EchoesOfEtherion.HealthSystem;

namespace EchoesOfEtherion.Player.Components
{
    [RequireComponent(typeof(PlayerMovement), typeof(PlayerAnimations))]
    [RequireComponent(typeof(PlayerInteractor))]
    [RequireComponent(typeof(PlayerSpellCaster))]
    [RequireComponent(typeof(HealthModule))]
    [RequireComponent(typeof(PlayerInventory))]
    public class PlayerController : TickRegistor
    {
        [field: SerializeField]
        public InputReader PlayerInput { get; private set; }
        [field: Space]

        public PlayerAnimations Animator { get; private set; }
        public PlayerMovement Movement { get; private set; }
        public PlayerInteractor Interactor { get; private set; }

        public PlayerSpellCaster SpellCaster { get; private set; }
        public PlayerInventory Inventory { get; private set; }

        public FiniteStateMachine<PlayerController> StateMachine { get; private set; }

        public Vector2 LookDirection
        {
            get
            {
                Vector2 pointerPos = Pointer.current != null ?
                    CameraController.Instance?.GameCamera.ScreenToWorldPoint(Pointer.current.position.ReadValue()) ?? Vector2.zero
                    : Vector2.zero;

                return (pointerPos != Vector2.zero ?
                    pointerPos - (Vector2)transform.position : Vector2.zero).normalized;
            }
        }

        public float StunTime { get; private set; }

        private HealthModule healthSystem;

        public bool IsDead => healthSystem.IsDead;

        private void Awake()
        {
            Animator ??= GetComponent<PlayerAnimations>();
            Movement ??= GetComponent<PlayerMovement>();
            Interactor ??= GetComponent<PlayerInteractor>();
            SpellCaster ??= GetComponent<PlayerSpellCaster>();
            Inventory ??= GetComponent<PlayerInventory>();
            healthSystem ??= GetComponent<HealthModule>();

            SetupStateMachine();
        }

        protected override void Start()
        {
            base.Start();

            try
            {
                CameraController.Instance?.CameraFollow.SetTarget(transform);
            }
            catch (Exception ex)
            {
                Debug.Log($"[PlayerController] {ex.Message}");
            }
        }

        private void OnEnable()
        {
            healthSystem.Died += OnDied;
            healthSystem.Damaged += OnDamaged;
        }

        private void OnDisable()
        {
            healthSystem.Died -= OnDied;
            healthSystem.Damaged -= OnDamaged;
        }

        public override void Tick() => StateMachine?.Update();
        public override void FixedTick() => StateMachine?.FixedUpdate();

        private void SetupStateMachine()
        {
            StateMachine = new FiniteStateMachine<PlayerController>(this);

            StateMachine.AddState<PlayerRoamingState>(new PlayerRoamingState());
            StateMachine.AddState<PlayerDeadState>(new PlayerDeadState());
            StateMachine.AddState<PlayerStunState>(new PlayerStunState());

            // Cheat states
            StateMachine.AddState<PlayerNoClipState>(new PlayerNoClipState());


            // Start with Roaming.
            StateMachine.ChangeState<PlayerRoamingState>();
        }

        private void OnDied(HealthModule module)
        {
            StateMachine.ChangeState<PlayerDeadState>();
        }

        private void OnDamaged(DamageInfo damageInfo)
        {
            if (IsDead) return;
            float stun = damageInfo.StunTime;
            if (stun > 0)
            {
                StunTime = stun;
                StateMachine.ChangeState<PlayerStunState>();
            }

            if (damageInfo.KnockbackAmount > 0)
            {
                Vector2 source = damageInfo.DamageSourcePos;
                Vector2 here = transform.position;
                Vector2 sourceToHere = here - source;
                Movement.RB.AddForce(sourceToHere.normalized * damageInfo.KnockbackAmount, ForceMode2D.Impulse);
            }
            Animator.Damage();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            Animator ??= GetComponent<PlayerAnimations>();
            Movement ??= GetComponent<PlayerMovement>();
            Interactor ??= GetComponent<PlayerInteractor>();
            SpellCaster ??= GetComponent<PlayerSpellCaster>();
            healthSystem ??= GetComponent<HealthModule>();
        }
#endif
    }
}