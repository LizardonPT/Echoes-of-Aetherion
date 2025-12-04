using EchoesOfEtherion.Enemies.EnemiesStateMachine;
using EchoesOfEtherion.Enemies.EnemiesStateMachine.Conditions;
using EchoesOfEtherion.Enemies.EnemiesStateMachine.States;
using EchoesOfEtherion.Player.Components;
using UnityEngine;

namespace EchoesOfEtherion.Enemies.StoneScorpion.States
{
    public class OrbitState : BaseState
    {
        [Header("Orbit Settings")]
        [SerializeField] private float orbitDistance = 60f;
        [SerializeField] private float offset = 25f;
        [SerializeField] private float orbitStrength = 1f;
        [SerializeField] private float tolerance = 2f;
        [SerializeField] private float minChangeDirectionTime = 2f;
        [SerializeField] private float maxChangeDirectionTime = 5f;
        [SerializeField] private float maxDistanceMultiplier = 1.5f;
        [SerializeField] private float angleThreshold = 90f;
        [SerializeField] private float raycastRange = 150f;
        [SerializeField] private RangeCondition InRange;
        [SerializeField] private RangeCondition OutOfRange;

        private int orbitDirection = 1;
        private float startTime;
        private float randomChangeTime;
        private float actualOrbitDistance;
        private StoneScorpionController scorpion;

        public int OrbitDirection
        {
            get => orbitDirection;
            set => orbitDirection = Mathf.Clamp(value, -1, 1);
        }

        protected override void OnInitialize()
        {
            scorpion = agent as StoneScorpionController;

            actualOrbitDistance = orbitDistance + Random.Range(-offset, offset);

            InRange.SetRange(actualOrbitDistance);
            OutOfRange.SetRange(actualOrbitDistance + tolerance);
        }

        public override void OnEnter()
        {
            base.OnEnter();

            orbitDirection = Random.Range(0, 2) == 0 ? 1 : -1;
            randomChangeTime = Random.Range(minChangeDirectionTime, maxChangeDirectionTime);
            startTime = Time.time;

            if (agent.Target != null)
            {
                Vector2 dirToTarget = (agent.TargetPos - (Vector2)agent.transform.position).normalized;
                agent.LookDirection = dirToTarget;
            }
        }

        public override void OnUpdate()
        {
            if (!ValidateTarget())
            {
                return;
            }

            if (Time.time - startTime >= randomChangeTime)
            {
                orbitDirection *= -1;
                randomChangeTime = Random.Range(minChangeDirectionTime, maxChangeDirectionTime);
                startTime = Time.time;
            }

            if (agent.Target != null)
            {
                Vector2 dirToTarget = (agent.TargetPos - (Vector2)agent.transform.position).normalized;
                agent.LookDirection = dirToTarget;

                if (scorpion != null)
                {
                    scorpion.Animator.UpdateAnimation(agent.RB.linearVelocity, dirToTarget);
                }
            }
        }

        public override void OnFixedUpdate()
        {
            if (agent.Target == null) return;

            Vector2 steering = CalculateOrbitSteering();

            Vector2 targetPosition = (Vector2)agent.transform.position + steering.normalized * 50f;

            agent.MoveToPosition(targetPosition, 1f);
        }

        private Vector2 CalculateOrbitSteering()
        {
            Vector2 targetPos = agent.TargetPos;
            Vector2 toTarget = targetPos - (Vector2)agent.transform.position;
            float distance = toTarget.magnitude;
            Vector2 dirToTarget = toTarget.normalized;

            Vector2 tangent = new Vector2(-dirToTarget.y * orbitDirection, dirToTarget.x * orbitDirection) * orbitStrength;

            Vector2 radialCorrection = Vector2.zero;
            if (distance > actualOrbitDistance + tolerance)
            {
                radialCorrection = dirToTarget * 0.3f; // Move toward target if too far
            }
            else if (distance < actualOrbitDistance - tolerance)
            {
                radialCorrection = -dirToTarget * 0.3f; // Move away from target if too close
            }

            // Combine tangent and radial forces
            return (tangent + radialCorrection).normalized * agent.RB.linearVelocity.magnitude;
        }

        private bool ValidateTarget()
        {
            if (agent.Target == null)
            {
                return false;
            }

            float distance = Vector2.Distance(agent.transform.position, agent.Target.transform.position);

            if (distance > actualOrbitDistance * maxDistanceMultiplier)
            {
                return false;
            }

            Vector2 origin = agent.transform.position;
            Vector2 dirToTarget = (agent.TargetPos - origin).normalized;

            if (Vector2.Angle(agent.LookDirection, dirToTarget) > angleThreshold)
            {
                return false;
            }

            LayerMask rayMask = (agent.PlayerMask | agent.EnvironmentMask) & ~agent.EnemyMask;
            RaycastHit2D rayHit = Physics2D.Raycast(origin, dirToTarget, raycastRange, rayMask);

            if (rayHit.collider == null ||
                !rayHit.collider.TryGetComponent(out PlayerController player) ||
                player != agent.Target)
            {
                return false;
            }

            return true;
        }

#if UNITY_EDITOR
        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();

            if (!Application.isPlaying || !enabled || agent.Target == null) return;

            Vector2 targetPos = agent.TargetPos;
            Vector2 currentPos = agent.transform.position;

            // Draw actual orbit distance with tolerance zone
            Gizmos.color = new Color(0.5f, 0.5f, 1f, 0.3f);
            Gizmos.DrawWireSphere(targetPos, actualOrbitDistance);

            // Draw tolerance boundaries
            Gizmos.color = new Color(0.3f, 0.3f, 0.8f, 0.2f);
            Gizmos.DrawWireSphere(targetPos, actualOrbitDistance - tolerance);
            Gizmos.DrawWireSphere(targetPos, actualOrbitDistance + tolerance);

            // Draw current distance to target
            float currentDistance = Vector2.Distance(currentPos, targetPos);
            Gizmos.color = currentDistance > actualOrbitDistance + tolerance ? Color.red :
                          currentDistance < actualOrbitDistance - tolerance ? Color.blue : Color.green;
            Gizmos.DrawLine(currentPos, targetPos);

            // Draw orbit direction indicator
            Vector2 toTarget = targetPos - currentPos;
            Vector2 dirToTarget = toTarget.normalized;
            Vector2 tangent = new Vector2(-dirToTarget.y * orbitDirection, dirToTarget.x * orbitDirection);

            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(currentPos, tangent * 30f);

            // Calculate and draw the current steering force
            Vector2 steering = CalculateOrbitSteering();
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(currentPos, steering.normalized * 30f);
        }
#endif
    }
}
