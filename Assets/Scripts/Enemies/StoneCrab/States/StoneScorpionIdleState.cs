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
            if (!controller.Target)
                controller.Target = Physics2D.OverlapCircle(controller.transform.position, controller.DetectionRadius, controller.PlayerMask)
                    ?.GetComponent<PlayerController>()?.gameObject;

            if (controller.Target)
            {
                controller.CameraFollow?.AddTarget(controller.transform);
                controller.StateMachine.ChangeState<StoneScorpionChaseState>();
            }
        }

        public void FixedUpdate(StoneScorpionController controller) { }

        public void Exit(StoneScorpionController controller) { }
    }
}
