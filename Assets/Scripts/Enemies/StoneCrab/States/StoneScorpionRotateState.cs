using UnityEngine;
using EchoesOfEtherion.StateMachine;

namespace EchoesOfEtherion.Enemies.StoneScorpion.States
{
    public class StoneScorpionRotateState : IState<StoneScorpionController>
    {
        private float timer;
        private float randomChangeTime;

        public void Enter(StoneScorpionController controller)
        {
            controller.SeekBehaviour.IsActive = false;

            controller.OrbitBehaviour.IsActive = true;
            controller.OrbitBehaviour.OrbitDirection = Random.Range(0, 2) == 0 ? 1 : -1;

            controller.StopBehaviour.IsActive = false;
            controller.ObstacleAvoidanceBehaviour.IsActive = true;
            controller.SeparationBehaviour.IsActive = true;

            randomChangeTime = Random.Range(1f, 5f);
            timer = 0;
        }

        public void Update(StoneScorpionController controller)
        {
            if (!ValidateTarget(controller)) return;

            controller.SetLastSeenPosition(controller.TargetPos);

            timer += Time.deltaTime;
            if (timer > randomChangeTime)
            {
                timer = 0;
                randomChangeTime = Random.Range(1f, 5f);
                controller.OrbitBehaviour.OrbitDirection *= -1;
            }

            Vector2 dirToTarget = (controller.TargetPos - (Vector2)controller.transform.position).normalized;
            controller.LookDirection = dirToTarget;

            controller.Animator.UpdateAnimation(controller.Velocity, dirToTarget);
        }

        public void FixedUpdate(StoneScorpionController controller) { }

        public void Exit(StoneScorpionController controller) { }

        private bool ValidateTarget(StoneScorpionController controller)
        {
            if (controller.Target == null)
            {
                controller.StateMachine.ChangeState<StoneScorpionSearchState>();
                return false;
            }

            float distance = Vector2.Distance(controller.transform.position, controller.Target.transform.position);

            if (distance > controller.OrbitBehaviour.OrbitDistance * 1.5f)
            {
                controller.StateMachine.ChangeState<StoneScorpionChaseState>();
                return false;
            }

            Vector2 origin = controller.transform.position;
            Vector2 dirToTarget = controller.TargetPos - origin;

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
