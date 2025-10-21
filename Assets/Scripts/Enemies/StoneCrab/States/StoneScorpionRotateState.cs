using UnityEngine;
using EchoesOfEtherion.StateMachine;

namespace EchoesOfEtherion.Enemies.StoneScorpion.States
{
    public class StoneScorpionRotateState : IState<StoneScorpionController>
    {
        private int rotateDirection;
        private float timer;
        private float randomChangeTime;

        private const float OrbitSpeed = 1f;
        private const float DistanceCorrection = 0.5f;
        private const float Tolerance = 2f;

        public void Enter(StoneScorpionController controller)
        {
            rotateDirection = Random.Range(0, 2) == 0 ? 1 : -1;
            randomChangeTime = Random.Range(1f, 5f);
            timer = 0;
        }

        public void Update(StoneScorpionController controller)
        {
            if (!ValidateTarget(controller)) return;

            timer += Time.deltaTime;
            if (timer > randomChangeTime)
            {
                timer = 0;
                randomChangeTime = Random.Range(1f, 5f);
                rotateDirection *= -1;
            }

            Vector2 pos = controller.transform.position;
            Vector2 targetPos = controller.TargetPos;
            Vector2 toTarget = targetPos - pos;
            float distance = toTarget.magnitude;
            Vector2 dirToTarget = toTarget.normalized;

            Vector2 tangent = new Vector2(-dirToTarget.y * rotateDirection, dirToTarget.x * rotateDirection) * OrbitSpeed;
            Vector2 radialCorrection = Vector2.zero;

            if (distance > controller.AttackDistance + Tolerance)
                radialCorrection = dirToTarget * DistanceCorrection;
            else if (distance < controller.AttackDistance - Tolerance)
                radialCorrection = -dirToTarget * DistanceCorrection;

            Vector2 moveDir = (tangent + radialCorrection).normalized;

            controller.Movement.UpdateMovement(moveDir);
            controller.LookDirection = dirToTarget;
            controller.Animator.UpdateAnimation(controller.Movement.Velocity, dirToTarget);
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

            float distance = Vector2.Distance(controller.transform.position, controller.Target.transform.position);

            if (distance > controller.AttackDistance * 1.5f)
            {
                controller.StateMachine.ChangeState<StoneScorpionChaseState>();
                return false;
            }

            Vector2 origin = controller.transform.position;
            Vector2 dirToTarget = controller.TargetPos - origin;

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
