using EchoesOfEtherion.Enemies.Core;
using EchoesOfEtherion.Enemies.SteeringBehaviours;
using EchoesOfEtherion.Enemies.StoneScorpion.States;
using EchoesOfEtherion.Player.Components;
using EchoesOfEtherion.StateMachine;
using FMODUnity;
using UnityEngine;
using UnityEngine.UIElements;

namespace EchoesOfEtherion.Enemies.StoneScorpion
{
    [RequireComponent(typeof(StoneScorpionAnimations))]
    [RequireComponent(typeof(SeekBehaviour))]
    [RequireComponent(typeof(StopBehaviour))]
    [RequireComponent(typeof(ObstacleAvoidanceBehaviour))]
    [RequireComponent(typeof(OrbitBehaviour))]
    [RequireComponent(typeof(SeparationBehaviour))]
    [RequireComponent(typeof(HealthSystem))]
    public class StoneScorpionController : Agent
    {
        [field: SerializeField] public float CoolDownTime { get; private set; } = 2;
        [Header("Attacks")]
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private Transform projectileSpawnPoint;
        [SerializeField] private float projectileDamage = 20f;

        public GameObject ProjectilePrefab => projectilePrefab;
        public float ProjectileDamage => projectileDamage;
        [SerializeField] private float stingAttackRange = 64f;
        [SerializeField] private float stingAttackRadius = 16f;
        [SerializeField] private float stingDamage = 25f;
        [SerializeField] private float attackCooldown = 3f;
        [SerializeField] private LayerMask playerDamageMask;
        [field: SerializeField] public EventReference RockThrow { get; private set; }
        [field: SerializeField] public EventReference GatherRock { get; private set; }
        [field: SerializeField] public EventReference Sting { get; private set; }
        [field: SerializeField] public EventReference Hit { get; private set; }
        public StoneScorpionAnimations Animator { get; private set; }
        public FiniteStateMachine<StoneScorpionController> StateMachine { get; private set; }
        public SeekBehaviour SeekBehaviour { get; private set; }
        public StopBehaviour StopBehaviour { get; private set; }
        public OrbitBehaviour OrbitBehaviour { get; private set; }
        public ObstacleAvoidanceBehaviour ObstacleAvoidanceBehaviour { get; private set; }
        public SeparationBehaviour SeparationBehaviour { get; private set; }

        private GameObject fakeTarget;
        public GameObject LastSeenTarget { get; private set; }
        private float lastAttackTime = 0;
        public bool CanAttack => Time.time >= lastAttackTime + attackCooldown;
        public float StingAttackRange => stingAttackRange;
        public float StingAttackRadius => stingAttackRadius;
        public float StingDamage => stingDamage;
        public LayerMask PlayerDamageMask => playerDamageMask;
        public Transform ProjectileSpawnPoint => projectileSpawnPoint;
        private HealthSystem healthSystem;

        protected override void Awake()
        {
            base.Awake();

            Animator = GetComponent<StoneScorpionAnimations>();
            SeekBehaviour = GetComponent<SeekBehaviour>();
            StopBehaviour = GetComponent<StopBehaviour>();
            OrbitBehaviour = GetComponent<OrbitBehaviour>();
            ObstacleAvoidanceBehaviour = GetComponent<ObstacleAvoidanceBehaviour>();
            SeparationBehaviour = GetComponent<SeparationBehaviour>();
            healthSystem = GetComponent<HealthSystem>();

            CreateFakeTarget();
            SetupStateMachine();
        }

        private void OnDied()
        {
            Destroy(gameObject);
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

        public override void Tick()
        {
            base.Tick();
            StateMachine.Update();
        }

        public override void FixedTick()
        {
            base.FixedTick();
            StateMachine.FixedUpdate();
        }

        private void CreateFakeTarget()
        {
            fakeTarget = new GameObject("LastSeenPosition")
            {
                hideFlags = HideFlags.HideInHierarchy
            };
            LastSeenTarget = fakeTarget;
        }

        public void SetLastSeenPosition(Vector2 position)
        {
            if (fakeTarget != null)
            {
                fakeTarget.transform.position = position;
            }
        }

        public void ClearLastSeenPosition()
        {
            if (fakeTarget != null)
            {
                fakeTarget.transform.position = new Vector3(9999, 9999, 9999);
            }
        }

        private void OnDamaged(float damage)
        {
            StateMachine.ChangeState<StoneScorpionDamagedState>();
        }

        private void SetupStateMachine()
        {
            StateMachine = new FiniteStateMachine<StoneScorpionController>(this);
            StateMachine.AddState<StoneScorpionIdleState>(new StoneScorpionIdleState());
            StateMachine.AddState<StoneScorpionChaseState>(new StoneScorpionChaseState());
            StateMachine.AddState<StoneScorpionRotateState>(new StoneScorpionRotateState());
            StateMachine.AddState<StoneScorpionSearchState>(new StoneScorpionSearchState());
            StateMachine.AddState<StoneScorpionDamagedState>(new StoneScorpionDamagedState());
            StateMachine.AddState<StoneScorpionProjectileAttackState>(new StoneScorpionProjectileAttackState());
            StateMachine.AddState<StoneScorpionStingAttackState>(new StoneScorpionStingAttackState());
            StateMachine.ChangeState<StoneScorpionIdleState>();
        }

        public void PerformStingAttack()
        {
            // Circle cast to detect players in sting range
            Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(
                transform.position + (Vector3)LookDirection * stingAttackRange,
                stingAttackRadius,
                playerDamageMask
            );

            foreach (Collider2D playerCollider in hitPlayers)
            {
                HealthSystem playerHealth = playerCollider.GetComponent<HealthSystem>();
                if (playerHealth != null)
                {
                    playerHealth.Damage(stingDamage);
                }
            }
        }

        public void ResetAttackCooldown()
        {
            lastAttackTime = Time.time;
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (fakeTarget != null)
            {
                Destroy(fakeTarget);
            }
        }
    }
}
