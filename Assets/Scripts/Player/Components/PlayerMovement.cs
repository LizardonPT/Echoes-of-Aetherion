using UnityEngine;

namespace EchoesOfAetherion.Player.Components
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float maxSpeed = 80f;
        [SerializeField] private float acceleration = 15f;
        [SerializeField] private float friction = 8f;
        [SerializeField] private float stopSpeed = 2f;

        public bool IsMoving => rb.linearVelocity.sqrMagnitude > 1e-5f;

        private Rigidbody2D rb;
        private Vector2 accumulatedForce;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        public void UpdateMovement(Vector2 movementInput)
        {
            movementInput = Vector2.ClampMagnitude(movementInput, 1f);
            accumulatedForce = Vector2.zero;

            if (movementInput.sqrMagnitude > 0.01f)
            {
                Accelerate(movementInput.normalized, movementInput.magnitude * maxSpeed, acceleration);
            }

            ApplyFriction();
            ApplyVelocity();
        }

        private void Accelerate(Vector2 wishDir, float wishSpeed, float accel)
        {
            float currentSpeed = rb.linearVelocity.magnitude;
            float addSpeed = wishSpeed - currentSpeed;

            if (addSpeed <= 0) return;

            float accelSpeed = accel * Time.fixedDeltaTime * wishSpeed;
            accelSpeed = Mathf.Min(accelSpeed, addSpeed);

            accumulatedForce += wishDir * accelSpeed;
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

            float control = speed < stopSpeed ? stopSpeed : speed;
            float drop = control * friction * Time.fixedDeltaTime;
            float newSpeed = Mathf.Max(speed - drop, 0);

            rb.linearVelocity = velocity * (newSpeed / speed);
        }

        private void ApplyVelocity()
        {
            if (accumulatedForce.sqrMagnitude > 0.01f)
            {
                rb.AddForce(accumulatedForce, ForceMode2D.Impulse);
            }
        }
    }
}