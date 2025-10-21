using UnityEngine;
using EchoesOfEtherion.StateMachine;

namespace EchoesOfEtherion.Enemies.StoneScorpion.States
{
    public class StoneScorpionChaseState : IState<StoneScorpionController>
    {
        public void Enter(StoneScorpionController controller) { }

        public void Update(StoneScorpionController controller)
        {
            if (!ValidateTarget(controller)) return;

            Vector2 dir = (controller.TargetPos - (Vector2)controller.transform.position).normalized;
            controller.LookDirection = dir;

            controller.Movement.UpdateMovement(dir);
            controller.Animator.UpdateAnimation(controller.Movement.Velocity, dir);
        }

        public void FixedUpdate(StoneScorpionController controller) { }
        public void Exit(StoneScorpionController controller) { }

        private bool ValidateTarget(StoneScorpionController controller)
        {
            if (controller.Target == null)
            {
                controller.StateMachine.ChangeState<StoneScorpionIdleState>();
                return false;
            }

            Vector2 origin = controller.transform.position;
            Vector2 dirToTarget = controller.TargetPos - origin;
            float distance = dirToTarget.magnitude;

            if (distance > controller.DetectionRadius + 16)
            {
                controller.Target = null;
                controller.StateMachine.ChangeState<StoneScorpionIdleState>();
                return false;
            }

            if (distance < controller.AttackDistance)
            {
                controller.StateMachine.ChangeState<StoneScorpionRotateState>();
                return false;
            }

            if (Vector2.Angle(controller.LookDirection, dirToTarget) > controller.LookAngle)
            {
                controller.StateMachine.ChangeState<StoneScorpionIdleState>();
                return false;
            }

            LayerMask rayMask = (controller.PlayerMask | controller.EnvironmentMask) & ~controller.EnemyMask;
            RaycastHit2D rayHit = Physics2D.Raycast(origin, dirToTarget.normalized, controller.DetectionRadius, rayMask);

            if (rayHit.collider == null || rayHit.collider.gameObject != controller.Target)
            {
                controller.StateMachine.ChangeState<StoneScorpionIdleState>();
                return false;
            }

            return true;
        }
    }
}
