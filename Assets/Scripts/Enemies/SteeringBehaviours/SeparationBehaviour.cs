using UnityEngine;

namespace EchoesOfEtherion.Enemies.SteeringBehaviours
{
    public class SeparationBehaviour : SteeringBehaviour
    {
        [Header("Separation Settings")]
        [SerializeField] private float separationRadius = 90f;

#if UNITY_EDITOR
        [Header("Debug")]
        [SerializeField] private bool showDebugCircles = false;
#endif

        public override Vector2 GetSteering(GameObject target)
        {
            Vector2 linear = Vector2.zero;
            Vector2 separationForce = CalculateSeparationForce();

            if (separationForce != Vector2.zero)
            {
                linear = separationForce * MaxAccel;
            }

            return linear;
        }

        private Vector2 CalculateSeparationForce()
        {
            Vector2 separationForce = Vector2.zero;
            int neighborCount = 0;

            Collider2D[] neighbors = Physics2D.OverlapCircleAll(
                transform.position,
                separationRadius,
                agent.EnemyMask
            );

            foreach (Collider2D neighbor in neighbors)
            {
                if (neighbor.gameObject == gameObject) continue;

                Vector2 toNeighbor = (Vector2)transform.position - (Vector2)neighbor.transform.position;
                float distance = toNeighbor.magnitude;

                if (distance > 0 && distance < separationRadius)
                {
                    float distanceFactor = Mathf.Lerp(1f, 0f, distance / separationRadius);
                    Vector2 force = toNeighbor.normalized * (distanceFactor * agent.MaxAccel);
                    separationForce += force;
                    neighborCount++;
                }
            }

            if (neighborCount > 0)
            {
                separationForce /= neighborCount;
            }
            return separationForce;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (showDebugCircles)
            {
                Gizmos.color = Color.aliceBlue;
                Gizmos.DrawWireSphere(transform.position, separationRadius);
            }
        }
#endif
    }
}
