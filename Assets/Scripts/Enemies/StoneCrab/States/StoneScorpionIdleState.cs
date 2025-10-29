using UnityEngine;
using EchoesOfEtherion.StateMachine;
using EchoesOfEtherion.Player.Components;

namespace EchoesOfEtherion.Enemies.StoneScorpion.States
{
    public class StoneScorpionIdleState : IState<StoneScorpionController>
    {
        public void Enter(StoneScorpionController controller)
        {
            controller.SeekBehaviour.IsActive = false;
            controller.OrbitBehaviour.IsActive = false;
            controller.StopBehaviour.IsActive = true;
            controller.ObstacleAvoidanceBehaviour.IsActive = false;
            controller.SeparationBehaviour.IsActive = false;
        }

        public void Update(StoneScorpionController controller)
        {
            if (DetectPlayer(controller))
                controller.StateMachine.ChangeState<StoneScorpionChaseState>();

            controller.Animator.UpdateAnimation(controller.Velocity, controller.LookDirection);
        }

        public void FixedUpdate(StoneScorpionController controller) { }

        public void Exit(StoneScorpionController controller) { }

        private bool DetectPlayer(StoneScorpionController controller)
        {
            if (controller.Target != null)
                return true;

            Vector2 origin = controller.transform.position;
            float radius = controller.DetectionRadius;

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
    }
}