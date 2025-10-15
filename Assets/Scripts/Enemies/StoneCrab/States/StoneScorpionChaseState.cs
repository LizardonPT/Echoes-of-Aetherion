using UnityEngine;
using EchoesOfAetherion.StateMachine;
using EchoesOfAetherion.Player.Components;

namespace EchoesOfAetherion.Enemies.StoneScorpion.States
{
    public class StoneScorpionChaseState : IState<StoneScorpionController>
    {
        public void Enter(StoneScorpionController controller) { }

        public void Update(StoneScorpionController controller)
        {
            if (Vector2.Distance(
                controller.transform.position,
                controller.Target.transform.position) > controller.DetectionRadius + 16)
            {
                controller.CameraFollow?.RemoveTarget(controller.transform);
                controller.Target = null;
                controller.StateMachine.ChangeState<StoneScorpionIdleState>();
            }
            else
            {
                Vector2 dir = (Vector2)(controller.Target.transform.position - controller.transform.position).normalized;
                controller.Animator.UpdateAnimation(dir, dir);
            }
        }

        public void FixedUpdate(StoneScorpionController controller) { }

        public void Exit(StoneScorpionController controller) { }
    }
}
