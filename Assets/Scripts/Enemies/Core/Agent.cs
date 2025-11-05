using EchoesOfEtherion.Enemies.SteeringBehaviours;
using EchoesOfEtherion.Extentions;
using EchoesOfEtherion.Game;
using UnityEngine;

namespace EchoesOfEtherion.Enemies.Core
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Agent : TickRegistor
    {
        [Header("Movement Settings")]
        [SerializeField] protected float maxAccel = 15f;
        [SerializeField] protected float maxSpeed = 65f;
        [SerializeField] protected float friction = 8f;

        [Header("Detection Settings")]
        [field: SerializeField] public LayerMask PlayerMask { get; private set; }
        [field: SerializeField] public LayerMask EnemyMask { get; private set; }
        [field: SerializeField] public LayerMask EnvironmentMask { get; private set; }
        [field: SerializeField] public float DetectionRadius { get; private set; } = 120f;
        [field: SerializeField] public float SeekRadius { get; private set; } = 180f;
        [field: SerializeField, Range(0, 360)] public int LookAngle { get; private set; } = 45;

        private ISteeringBehaviour[] steeringBehaviours;
        private Rigidbody2D rb;

        public float MaxAccel => maxAccel;
        public float MaxSpeed => maxSpeed;
        public Vector2 Velocity => rb.linearVelocity;
        public Vector2 LookDirection = Vector2.right;
        public GameObject Target { get; set; }
        public Vector2 TargetPos => Target != null ? new Vector2(Target.transform.position.x, Target.transform.position.y + 6) : Vector2.zero;

        protected virtual void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            steeringBehaviours = GetComponents<ISteeringBehaviour>();
        }

        public override void FixedTick()
        {
            ApplySteering();
            ApplyFriction();
        }

        private void ApplySteering()
        {
            if (steeringBehaviours == null || steeringBehaviours.Length == 0)
                return;

            Vector2 steerWeighted = Vector2.zero;

            foreach (ISteeringBehaviour behaviour in steeringBehaviours)
            {
                if (!behaviour.IsActive) continue;

                Vector2 steer = behaviour.GetSteering(Target);
                steerWeighted += behaviour.Weight * steer;
            }

            steerWeighted = Vector2.ClampMagnitude(steerWeighted, maxAccel);
            rb.AddForce(steerWeighted, ForceMode2D.Impulse);

            if (rb.linearVelocity.magnitude > maxSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
            }
        }

        private void ApplyFriction()
        {
            Vector2 velocity = rb.linearVelocity;
            float speed = velocity.magnitude;

            if (speed < 0.01f)
            {
                rb.linearVelocity = Vector2.zero;
                return;
            }

            float drop = speed * friction * Time.fixedDeltaTime;
            float newSpeed = Mathf.Max(speed - drop, 0);

            rb.linearVelocity = velocity * (newSpeed / speed);
        }


#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Vector2 pos = transform.position;

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(pos, DetectionRadius);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(pos, SeekRadius);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(pos, pos + LookDirection.normalized * DetectionRadius);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(pos, pos + LookDirection.Rotate(LookAngle).normalized * DetectionRadius);
            Gizmos.DrawLine(pos, pos + LookDirection.Rotate(-LookAngle).normalized * DetectionRadius);
        }
#endif
    }
}