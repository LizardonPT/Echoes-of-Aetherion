using UnityEngine;

namespace EchoesOfEtherion.Game
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float maxSpeed = 80f;
        [SerializeField] private float acceleration = 15f;
        [SerializeField] private float friction = 8f;

        public bool IsMoving => RB.linearVelocity.sqrMagnitude > 1e-5f;
        public Vector2 Velocity => RB.linearVelocity;
        public float Speed => RB.linearVelocity.magnitude;

        public Rigidbody2D RB { get; private set; }

        private void Awake()
        {
            RB = GetComponent<Rigidbody2D>();
        }

        public void UpdateMovement(Vector2 movementInput)
        {
            movementInput = movementInput.normalized;
            Vector2 accumulatedForce = Vector2.zero;

            if (movementInput.sqrMagnitude > 1e-5)
            {
                accumulatedForce += Accelerate(movementInput.normalized, movementInput.magnitude * maxSpeed, acceleration);
            }

            ApplyFriction();
            ApplyVelocity(accumulatedForce);
        }

        private Vector2 Accelerate(Vector2 wishDir, float wishSpeed, float accel)
        {
            float currentSpeed = RB.linearVelocity.magnitude;
            float addSpeed = wishSpeed - currentSpeed;

            if (addSpeed <= 0) return Vector2.zero;

            float accelSpeed = accel * Time.fixedDeltaTime * wishSpeed;
            accelSpeed = Mathf.Min(accelSpeed, addSpeed);

            return wishDir * accelSpeed;
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

        private void ApplyVelocity(Vector2 accumulatedForce)
        {
            if (accumulatedForce.sqrMagnitude > 0.01f)
            {
                RB.AddForce(accumulatedForce, ForceMode2D.Impulse);
            }
        }
    }
}