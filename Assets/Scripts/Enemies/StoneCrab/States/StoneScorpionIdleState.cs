using UnityEngine;
using EchoesOfAetherion.StateMachine;
using EchoesOfAetherion.Player.Components;

namespace EchoesOfAetherion.Enemies.StoneScorpion.States
{
    public class StoneScorpionIdleState : IState<StoneScorpionController>
    {
        public void Enter(StoneScorpionController controller) { }

        public void Update(StoneScorpionController controller)
        {
            if (controller.Target == null)
            {
                Vector3 origin = controller.transform.position;
                float radius = controller.DetectionRadius;
                LayerMask mask = controller.PlayerMask;

                var hit = Physics2D.OverlapCircle(origin, radius, mask);
                if (hit != null)
                {
                    if (hit.TryGetComponent<PlayerController>(out var player))
                        controller.Target = player.gameObject;
                }
            }

            if (controller.Target != null)
            {
                controller.StateMachine.ChangeState<StoneScorpionChaseState>();
            }
        }

        public void FixedUpdate(StoneScorpionController controller) { }

        public void Exit(StoneScorpionController controller) { }
    }
}
