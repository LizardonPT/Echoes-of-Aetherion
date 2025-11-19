using UnityEngine;

namespace EchoesOfEtherion.Enemies.SteeringBehaviours
{
    public class StopBehaviour : SteeringBehaviour
    {
        public override Vector2 GetSteering(GameObject target)
        {
            if (Velocity.magnitude > 0.1f)
            {
                Vector2 oppositeForce = -Velocity;
                return Vector2.ClampMagnitude(oppositeForce, agent.MaxAccel);
            }

            return Vector2.zero;
        }
    }
}
