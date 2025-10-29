using UnityEngine;

namespace EchoesOfEtherion.Enemies.SteeringBehaviours
{
    public class ObstacleAvoidanceBehaviour : SteeringBehaviour
    {
        [SerializeField] private float distanceCheck = 60f;

#if UNITY_EDITOR
        [SerializeField] private bool showDebugGizmos = false;
        private float[] rayLengths;
        private Vector2[] hitPoints;
        private bool[] rayHitFlags;
#endif

        private Vector2[] directions;

        private int bestDirectionIndex = -1;
        private Vector2 finalSteering;

        private void Awake()
        {
            InitializeDirections();
        }

        private void InitializeDirections()
        {
            directions = new Vector2[16];
            for (int i = 0; i < 16; i++)
            {
                float angle = i * 22.5f * Mathf.Deg2Rad;
                directions[i] = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;
            }

#if UNITY_EDITOR
            rayLengths = new float[directions.Length];
            hitPoints = new Vector2[directions.Length];
            rayHitFlags = new bool[directions.Length];
#endif
        }

        public override Vector2 GetSteering(GameObject target)
        {
            float[] undesirability = CalculateUndesirability(directions);
            bestDirectionIndex = GetBestDirectionIndex(undesirability);

            if (bestDirectionIndex < 0 || bestDirectionIndex >= directions.Length ||
                Mathf.Abs(undesirability[bestDirectionIndex]) < 1e-5f)
            {
                finalSteering = Vector2.zero;
                return Vector2.zero;
            }

            finalSteering = Vector2.ClampMagnitude(-directions[bestDirectionIndex] * undesirability[bestDirectionIndex], MaxAccel);
            return finalSteering;
        }

        private int GetBestDirectionIndex(float[] undesirability)
        {
            int bestIndex = 0;
            float bestUndesirability = undesirability[0];
            for (int i = 1; i < undesirability.Length; i++)
            {
                if (undesirability[i] > bestUndesirability)
                {
                    bestUndesirability = undesirability[i];
                    bestIndex = i;
                }
            }
            return bestIndex;
        }

        private float[] CalculateUndesirability(Vector2[] directions)
        {
            float[] undesirability = new float[directions.Length];

#if UNITY_EDITOR
            for (int i = 0; i < directions.Length; i++)
            {
                rayLengths[i] = distanceCheck;
                rayHitFlags[i] = false;
                hitPoints[i] = Vector2.zero;
            }
#endif

            for (int i = 0; i < directions.Length; i++)
            {
                RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, directions[i], distanceCheck, agent.EnvironmentMask);

#if UNITY_EDITOR
                float nearestDist = float.MaxValue;
                Vector2 nearestPoint = Vector2.zero;
                bool anyHit = false;
#endif

                foreach (var hit in hits)
                {
                    if (hit.collider == null) continue;

                    float distance = Vector2.Distance(transform.position, hit.point);

#if UNITY_EDITOR
                    if (distance < nearestDist)
                    {
                        nearestDist = distance;
                        nearestPoint = hit.point;
                        anyHit = true;
                    }
#endif

                    for (int j = 0; j < directions.Length; j++)
                    {
                        Vector2 obstacleDirection = (hit.point - (Vector2)transform.position).normalized;
                        float dotWithObstacle = Vector2.Dot(directions[j], obstacleDirection);
                        float distanceFactor = Mathf.Lerp(1f, 0f, distance / distanceCheck);
                        undesirability[j] += distanceFactor * dotWithObstacle * MaxAccel;
                    }
                }

#if UNITY_EDITOR
                if (anyHit)
                {
                    rayLengths[i] = Mathf.Clamp(nearestDist, 0f, distanceCheck);
                    hitPoints[i] = nearestPoint;
                    rayHitFlags[i] = true;
                }
                else
                {
                    rayLengths[i] = distanceCheck;
                    rayHitFlags[i] = false;
                }
#endif
            }


            return undesirability;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!showDebugGizmos) return;

            if (directions == null || directions.Length == 0)
                InitializeDirections();

            Vector3 origin = transform.position;

            if (!Application.isPlaying)
            {
                for (int i = 0; i < directions.Length; i++)
                {
                    RaycastHit2D hit = Physics2D.Raycast(origin, directions[i], distanceCheck, GetEditorEnvironmentMask());
                    if (hit.collider != null)
                    {
                        rayLengths[i] = Vector2.Distance(origin, hit.point);
                        hitPoints[i] = hit.point;
                        rayHitFlags[i] = true;
                    }
                    else
                    {
                        rayLengths[i] = distanceCheck;
                        rayHitFlags[i] = false;
                    }
                }
            }

            for (int i = 0; i < directions.Length; i++)
            {
                Vector3 dir = directions[i];
                Vector3 fullEnd = origin + dir * distanceCheck;

                Gizmos.color = Color.yellow * 0.6f;
                Gizmos.DrawLine(origin, fullEnd);

                if (rayHitFlags[i])
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(origin, origin + dir * rayLengths[i]);
                    Gizmos.DrawSphere(hitPoints[i], 0.2f);
                }
            }

            if (finalSteering != Vector2.zero)
            {
                Gizmos.color = Color.blue;
                Vector3 steeringEnd = origin + (Vector3)finalSteering;
                Gizmos.DrawLine(origin, steeringEnd);
                Gizmos.DrawSphere(steeringEnd, 0.3f);
            }
        }

        private int GetEditorEnvironmentMask()
        {
            try
            {
                if (agent != null) return agent.EnvironmentMask;
            }
            catch { }
            return ~0;
        }
#endif
    }
}
