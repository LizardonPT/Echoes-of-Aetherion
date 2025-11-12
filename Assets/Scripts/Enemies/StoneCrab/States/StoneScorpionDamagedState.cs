using UnityEngine;
using EchoesOfEtherion.StateMachine;
using EchoesOfEtherion.Player.Components;
using FMODUnity;

namespace EchoesOfEtherion.Enemies.StoneScorpion.States
{
    public class StoneScorpionDamagedState : IState<StoneScorpionController>
    {
        private float timer = 0;

        public void Enter(StoneScorpionController controller)
        {
            timer = controller.CoolDownTime;

            GameObject target = ValidateTarget(controller);

            if (target != null)
            {
                controller.Target = target;
            }
            controller.SeekBehaviour.IsActive = false;
            controller.OrbitBehaviour.IsActive = false;
            controller.StopBehaviour.IsActive = true;
            controller.ObstacleAvoidanceBehaviour.IsActive = false;
            controller.SeparationBehaviour.IsActive = false;

            RuntimeManager.PlayOneShot(controller.Hit, controller.transform.position);
        }

        public void Update(StoneScorpionController controller)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                if (controller.Target != null)
                {
                    controller.StateMachine.ChangeState<StoneScorpionChaseState>();
                }
                else controller.StateMachine.ChangeState<StoneScorpionIdleState>();
            }
        }

        public void FixedUpdate(StoneScorpionController controller) { }

        public void Exit(StoneScorpionController controller) { }

        private GameObject ValidateTarget(StoneScorpionController controller)
        {
            if (controller.Target != null)
            {
                return controller.Target;
            }

            var player = GameObject.FindAnyObjectByType<PlayerController>();

            Vector2 origin = controller.transform.position;
            Vector2 dirToTarget = (Vector2)player.transform.position - origin;
            LayerMask rayMask = (controller.PlayerMask | controller.EnvironmentMask) & ~controller.EnemyMask;
            RaycastHit2D rayHit = Physics2D.Raycast(origin, dirToTarget.normalized, controller.SeekRadius, rayMask);

            return rayHit.collider.gameObject;
        }
    }
}
