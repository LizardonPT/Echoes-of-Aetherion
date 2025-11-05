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

        public bool IsMoving => rb.linearVelocity.sqrMagnitude > 1e-5f;
        public Vector2 Velocity => rb.linearVelocity;
        public float Speed => rb.linearVelocity.magnitude;

        private Rigidbody2D rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
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
            float currentSpeed = rb.linearVelocity.magnitude;
            float addSpeed = wishSpeed - currentSpeed;

            if (addSpeed <= 0) return Vector2.zero;

            float accelSpeed = accel * Time.fixedDeltaTime * wishSpeed;
            accelSpeed = Mathf.Min(accelSpeed, addSpeed);

            return wishDir * accelSpeed;
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

        private void ApplyVelocity(Vector2 accumulatedForce)
        {
            if (accumulatedForce.sqrMagnitude > 0.01f)
            {
                rb.AddForce(accumulatedForce, ForceMode2D.Impulse);
            }
        }
    }
}