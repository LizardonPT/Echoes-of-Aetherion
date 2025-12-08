using EchoesOfEtherion.Enemies.EnemiesStateMachine;
using EchoesOfEtherion.Enemies.EnemiesStateMachine.States;
using EchoesOfEtherion.Game;
using EchoesOfEtherion.HealthSystem;
using EchoesOfEtherion.Player.Components;
using UnityEngine;

namespace EchoesOfEtherion.Enemies.Core
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(StateMachine))]
    [RequireComponent(typeof(HealthModule))]
    public class Agent : TickRegistor
    {
        public virtual string EnemyType { get; }
        [SerializeField] private bool showMovementGizmos = true;

        [Header("Movement Settings")]
        [SerializeField] protected float accel = 15f;
        [SerializeField] protected float maxSpeed = 65f;
        [SerializeField] protected float friction = 8f;

        [Header("Detection Settings")]
        [field: SerializeField] public LayerMask PlayerMask { get; private set; }
        [field: SerializeField] public LayerMask EnemyMask { get; private set; }
        [field: SerializeField] public LayerMask EnvironmentMask { get; private set; }
        [field: SerializeField] public float SignalRange { get; private set; } = 120f;

        public Rigidbody2D RB { get; private set; }

        public Vector2 LookDirection = Vector2.right;
        public PlayerController Target { get; set; }
        public float StunTime { get; private set; }
        public Vector2 TargetPos => Target != null ? new Vector2(Target.transform.position.x, Target.transform.position.y + 6) : Vector2.zero;

        private StateMachine stateMachine;
        private HealthModule healthModule;

        protected virtual void Awake()
        {
            RB = GetComponent<Rigidbody2D>();
            stateMachine = GetComponent<StateMachine>();
            healthModule = GetComponent<HealthModule>();
        }

        private void OnEnable()
        {
            healthModule.Died += OnDied;
            healthModule.Damaged += OnDamaged;
        }

        private void OnDisable()
        {
            healthModule.Died -= OnDied;
            healthModule.Damaged -= OnDamaged;
        }

        public override void FixedTick()
        {
            ApplyFriction();
        }

        public void MoveToPosition(Vector2 targetPosition, float speedMultiplier = 1f)
        {
            Vector2 origin = transform.position;
            Vector2 wishVelocity = accel * speedMultiplier * (targetPosition - origin).normalized;

            if (RB.linearVelocity.magnitude < maxSpeed * speedMultiplier)
            {
                float maxAddition = (maxSpeed * speedMultiplier) - RB.linearVelocity.magnitude;
                wishVelocity = Vector2.ClampMagnitude(wishVelocity, maxAddition);
            }

            wishVelocity = Vector2.ClampMagnitude(wishVelocity, accel);

            RB.AddForce(wishVelocity, ForceMode2D.Impulse);
        }

        private void ApplyFriction()
        {
            Vector2 velocity = RB.linearVelocity;
            float speed = velocity.magnitude;

            if (speed < 0.01f)
            {
                RB.linearVelocity = Vector2.zero;
                return;
            }

            float drop = speed * friction * Time.fixedDeltaTime;
            float newSpeed = Mathf.Max(speed - drop, 0);

            RB.linearVelocity = velocity * (newSpeed / speed);
        }

        public void SignalEnemyHit()
        {
            if (Target != null) return;

            var player = FindAnyObjectByType<PlayerController>();
            if (player == null)
                return;

            Vector2 origin = transform.position;
            Vector2 dirToTarget = (Vector2)player.transform.position - origin;

            LayerMask rayMask = (PlayerMask | EnvironmentMask) & ~EnemyMask;

            RaycastHit2D rayHit = Physics2D.Raycast(origin, dirToTarget.normalized, 500, rayMask);

            if (rayHit.collider != null)
            {
                if (rayHit.collider.TryGetComponent(out PlayerController playerController))
                {
                    Target = playerController;
                }
            }
        }

        private void OnDamaged(DamageInfo info)
        {
            StunTime = info.StunTime;

            if (info.KnockbackAmount > 0)
            {
                Vector2 source = info.DamageSourcePos;
                Vector2 here = transform.position;
                Vector2 sourceToHere = here - source;
                RB.AddForce(sourceToHere.normalized * info.KnockbackAmount, ForceMode2D.Impulse);
            }

            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, SignalRange, EnemyMask);

            foreach (Collider2D col in hits)
            {
                if (col.TryGetComponent(out Agent agent))
                {
                    agent.SignalEnemyHit();
                }
            }

            SignalEnemyHit();
        }

        private void OnDied(HealthModule module)
        {
            Destroy(gameObject);
        }


#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!showMovementGizmos || !Application.isPlaying) return;

            // Draw movement direction
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, RB.linearVelocity.normalized * 50);

            // Draw look direction
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, LookDirection * 50f);
        }
#endif
    }
}