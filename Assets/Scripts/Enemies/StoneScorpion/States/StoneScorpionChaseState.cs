using UnityEngine;
using EchoesOfEtherion.StateMachine;

namespace EchoesOfEtherion.Enemies.StoneScorpion.States
{
    public class StoneScorpionChaseState : IState<StoneScorpionController>
    {
        public void Enter(StoneScorpionController controller)
        {
            controller.SeekBehaviour.IsActive = true;
            controller.OrbitBehaviour.IsActive = false;
            controller.StopBehaviour.IsActive = false;
            controller.ObstacleAvoidanceBehaviour.IsActive = true;
            controller.SeparationBehaviour.IsActive = true;
        }

        public void Update(StoneScorpionController controller)
        {
            if (!ValidateTarget(controller)) return;

            controller.SetLastSeenPosition(controller.TargetPos);

            Vector2 dir = (controller.TargetPos - (Vector2)controller.transform.position).normalized;
            controller.LookDirection = dir;

            controller.Animator.UpdateAnimation(controller.Velocity, dir);
            
            if (controller.Target != null && controller.CanAttack)
            {
                float distanceToTarget = Vector2.Distance(controller.transform.position, controller.TargetPos);

                // Decide which attack to use based on distance
                if (distanceToTarget <= controller.StingAttackRange)
                {
                    // Close range - use sting attack
                    controller.StateMachine.ChangeState<StoneScorpionStingAttackState>();
                }
                else if (distanceToTarget <= controller.SeekRadius)
                {
                    // Medium range - use projectile attack
                    controller.StateMachine.ChangeState<StoneScorpionProjectileAttackState>();
                }
            }
        }

        public void FixedUpdate(StoneScorpionController agent) { }

        public void Exit(StoneScorpionController controller) { }

        private bool ValidateTarget(StoneScorpionController controller)
        {
            if (controller.Target == null)
            {
                controller.StateMachine.ChangeState<StoneScorpionSearchState>();
                return false;
            }

            Vector2 origin = controller.transform.position;
            Vector2 dirToTarget = controller.TargetPos - origin;
            float distance = dirToTarget.magnitude;

            controller.SetLastSeenPosition(controller.TargetPos);

            if (distance > controller.SeekRadius + 16)
            {
                controller.Target = null;
                controller.StateMachine.ChangeState<StoneScorpionSearchState>();
                return false;
            }

            if (distance < controller.OrbitBehaviour.OrbitDistance)
            {
                controller.StateMachine.ChangeState<StoneScorpionRotateState>();
                return false;
            }

            if (Vector2.Angle(controller.LookDirection, dirToTarget) > controller.LookAngle)
            {
                controller.Target = null;
                controller.StateMachine.ChangeState<StoneScorpionSearchState>();
                return false;
            }

            LayerMask rayMask = (controller.PlayerMask | controller.EnvironmentMask) & ~controller.EnemyMask;
            RaycastHit2D rayHit = Physics2D.Raycast(origin, dirToTarget.normalized, controller.SeekRadius, rayMask);

            if (rayHit.collider == null || rayHit.collider.gameObject != controller.Target)
            {
                controller.Target = null;
                controller.StateMachine.ChangeState<StoneScorpionSearchState>();
                return false;
            }

            return true;
        }
    }
}
