using UnityEngine;
using EchoesOfEtherion.StateMachine;
using EchoesOfEtherion.Player.Components;

namespace EchoesOfEtherion.Enemies.StoneScorpion.States
{
    public class StoneScorpionSearchState : IState<StoneScorpionController>
    {
        private bool reachedLastSeenPosition = false;

        public void Enter(StoneScorpionController controller)
        {
            controller.SeekBehaviour.IsActive = true;
            controller.Target = controller.LastSeenTarget;

            controller.StopBehaviour.IsActive = false;
            controller.ObstacleAvoidanceBehaviour.IsActive = true;
            controller.OrbitBehaviour.IsActive = false;
            controller.SeparationBehaviour.IsActive = true;

            reachedLastSeenPosition = false;
        }

        public void Update(StoneScorpionController controller)
        {
            if (CheckForPlayer(controller))
            {
                controller.ClearLastSeenPosition();
                controller.StateMachine.ChangeState<StoneScorpionChaseState>();
                return;
            }

            if (!reachedLastSeenPosition && HasReachedLastSeenPosition(controller))
            {
                reachedLastSeenPosition = true;

                controller.SeekBehaviour.IsActive = false;
                controller.StopBehaviour.IsActive = true;

                controller.ClearLastSeenPosition();
                controller.Target = null;
                controller.StateMachine.ChangeState<StoneScorpionIdleState>();
            }


            Vector2 lookDir = controller.Target != null ?
                (controller.TargetPos - (Vector2)controller.transform.position).normalized :
                controller.LookDirection;

            controller.LookDirection = lookDir;

            controller.Animator.UpdateAnimation(controller.Velocity, lookDir);
        }

        public void FixedUpdate(StoneScorpionController controller) { }

        public void Exit(StoneScorpionController controller)
        {
            controller.SeekBehaviour.IsActive = false;
        }

        private bool CheckForPlayer(StoneScorpionController controller)
        {
            Vector2 origin = controller.transform.position;
            float radius = controller.SeekRadius;

            Collider2D[] hits = Physics2D.OverlapCircleAll(origin, radius, controller.PlayerMask);
            foreach (var hit in hits)
            {
                if (!hit.TryGetComponent(out PlayerController player)) continue;

                Vector2 dirToTarget = (Vector2)(player.transform.position + new Vector3(0, 6)) - origin;

                if (Vector2.Angle(controller.LookDirection, dirToTarget) > controller.LookAngle)
                    continue;

                LayerMask rayMask = (controller.PlayerMask | controller.EnvironmentMask) & ~controller.EnemyMask;
                RaycastHit2D rayHit = Physics2D.Raycast(origin, dirToTarget.normalized, radius, rayMask);

                if (rayHit.collider != null && rayHit.collider.gameObject == player.gameObject)
                {
                    controller.Target = player.gameObject;
                    return true;
                }
            }

            return false;
        }

        private bool HasReachedLastSeenPosition(StoneScorpionController controller)
        {
            if (controller.Target != controller.LastSeenTarget) return false;

            float distance = Vector2.Distance(controller.transform.position, controller.TargetPos);
            return distance < 16;
        }
    }
}
