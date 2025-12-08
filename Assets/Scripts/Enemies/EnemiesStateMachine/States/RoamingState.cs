using UnityEngine;

namespace EchoesOfEtherion.Enemies.EnemiesStateMachine.States
{
    public class RoamingState : BaseState
    {
        [Header("Roaming Settings")]
        [SerializeField] private float roamSpeed = 0.3f;
        [SerializeField] private float minRoamTime = 1f;
        [SerializeField] private float maxRoamTime = 4f;
        [SerializeField] private float roamRadius = 100f;

        private Vector2 roamTarget;
        private float duration;
        private float startTime;

        protected override void OnInitialize()
        {

        }

        public override void OnEnter()
        {
            base.OnEnter();

            duration = Random.Range(minRoamTime, maxRoamTime);
            agent.Target = null;

            SetNewRoamTarget();
            StartTimer();
        }

        private void StartTimer()
        {
            startTime = Time.time;
        }

        public override void OnUpdate()
        {
            // Check if we reached the roam target
            if (Vector2.Distance(transform.position, roamTarget) < 5f)
            {
                SetNewRoamTarget();
            }

            // Update roam timer duration randomly
            if (Time.time - startTime >= duration)
            {
                duration = Random.Range(minRoamTime, maxRoamTime);
                StartTimer();
                SetNewRoamTarget();
            }
        }

        public override void OnFixedUpdate()
        {
            agent.MoveToPosition(roamTarget, roamSpeed);
        }

        private void SetNewRoamTarget()
        {
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            roamTarget = (Vector2)transform.position + randomDirection * roamRadius;

            agent.LookDirection = (roamTarget - (Vector2)agent.transform.position).normalized;
        }

#if UNITY_EDITOR
        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();
            
            if (!Application.isPlaying || !enabled) return;

            // Draw roam target and path
            Gizmos.color = new Color(1f, 1f, 0f, 0.5f);
            Gizmos.DrawWireSphere(roamTarget, 40f);
            Gizmos.DrawLine(transform.position, roamTarget);

            // Draw roam radius
            Gizmos.color = new Color(1f, 1f, 0f, 0.1f);
            Gizmos.DrawWireSphere(transform.position, roamRadius);
        }
#endif
    }
}
