using UnityEngine;
using EchoesOfEtherion.StateMachine;
using EchoesOfEtherion.Player.Components;

namespace EchoesOfEtherion.Enemies.StoneScorpion.States
{
    public class StoneScorpionIdleState : IState<StoneScorpionController>
    {
        public void Enter(StoneScorpionController controller) { }

        public void Update(StoneScorpionController controller)
        {
            if (DetectPlayer(controller))
                controller.StateMachine.ChangeState<StoneScorpionChaseState>();

            controller.Movement.UpdateMovement(Vector2.zero);
            controller.Animator.UpdateAnimation(Vector2.zero, controller.LookDirection);
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
