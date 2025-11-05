using UnityEngine;

namespace EchoesOfEtherion.Enemies.SteeringBehaviours
{
    public class StopBehaviour : SteeringBehaviour
    {
        [SerializeField] private float slowingRadius = 2f;

        public override Vector2 GetSteering(GameObject target)
        {
            if (Velocity.magnitude > 0.1f)
            {
                Vector2 oppositeForce = -Velocity.normalized * MaxAccel;

                if (Velocity.magnitude < slowingRadius)
                {
                    oppositeForce *= Velocity.magnitude / slowingRadius;
                }

                return Vector2.ClampMagnitude(oppositeForce, Velocity.magnitude);
            }

            return Vector2.zero;
        }

        private void OnValidate()
        {
            //! This is important since we shouldn't divide by 0.
            slowingRadius = slowingRadius <= 0 ? 1e-5f : slowingRadius;
        }
    }
}
