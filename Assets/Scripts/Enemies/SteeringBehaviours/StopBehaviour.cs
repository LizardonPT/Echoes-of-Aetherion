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
                Vector2 oppositeForce = -Velocity;
                return Vector2.ClampMagnitude(oppositeForce, agent.MaxAccel);
            }

            return Vector2.zero;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            //! This is important since we shouldn't divide by 0.
            slowingRadius = slowingRadius <= 0 ? 1e-5f : slowingRadius;
        }
#endif
    }
}
