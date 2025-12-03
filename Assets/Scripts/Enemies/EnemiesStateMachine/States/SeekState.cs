using UnityEngine;

namespace EchoesOfEtherion.Enemies.EnemiesStateMachine.States
{
    public class SeekState : BaseState
    {
        [Header("Seek Settings")]
        [SerializeField] private float seekSpeed = 1f;
        [SerializeField] private float predictionDistance = 20f;

        public override void OnEnter()
        {
            
        }

        public override void OnUpdate()
        {
            if (agent.Target == null)
            {
                ChangeState<RoamingState>();
                return;
            }

            Vector2 direction = (agent.TargetPos - (Vector2)transform.position).normalized;
            agent.LookDirection = direction;
        }

        public override void OnFixedUpdate()
        {
            if (agent.Target == null) return;

            // Simple prediction: aim slightly ahead of target
            Vector2 targetVelocity = agent.Target.GetComponent<Rigidbody2D>()?.linearVelocity ?? Vector2.zero;
            Vector2 predictedPosition = agent.TargetPos + targetVelocity.normalized * predictionDistance;

            agent.MoveToPosition(predictedPosition, seekSpeed);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying || !enabled || agent.Target == null) return;

            // Draw seek path
            Gizmos.color = new Color(1f, 0f, 0f, 0.7f);
            Gizmos.DrawLine(transform.position, agent.TargetPos);

            // Draw prediction
            Vector2 targetVelocity = agent.Target.GetComponent<Rigidbody2D>()?.linearVelocity ?? Vector2.zero;
            Vector2 predictedPosition = agent.TargetPos + targetVelocity.normalized * predictionDistance;

            Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f);
            Gizmos.DrawWireSphere(predictedPosition, 5f);
            Gizmos.DrawLine(agent.TargetPos, predictedPosition);
        }
#endif
    }
}
