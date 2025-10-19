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
            Vector3 pos = controller.transform.position;
            Vector3 targetPos = controller.Target.transform.position;
            float detectionRadius = controller.DetectionRadius + 16;

            if (Vector2.Distance(pos, targetPos) > detectionRadius)
            {
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
