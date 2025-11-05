using UnityEngine;

namespace EchoesOfEtherion.Enemies.SteeringBehaviours
{
    public class SeekBehaviour : SteeringBehaviour
    {
        public override Vector2 GetSteering(GameObject target)
        {
            Vector2 linear = Vector2.zero;

            if (target != null)
            {
                linear = target.transform.position - transform.position;

                linear = linear.normalized * MaxAccel;
            }

            return linear;
        }
    }
}
