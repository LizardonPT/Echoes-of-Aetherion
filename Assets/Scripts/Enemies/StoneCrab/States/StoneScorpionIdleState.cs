using UnityEngine;
using EchoesOfAetherion.StateMachine;

namespace EchoesOfAetherion.Enemies.StoneScorpion.States
{
    public class StoneScorpionIdleState : IState<StoneScorpionController>
    {
        public void Enter(StoneScorpionController controller) { }

        public void Update(StoneScorpionController controller)
        {
            if (controller.Data.Target != null)
            {
                Vector2 dir = (Vector2)(controller.Data.Target.position - controller.transform.position).normalized;
                controller.Animator.UpdateAnimation(Vector2.zero, dir);
            }
        }

        public void FixedUpdate(StoneScorpionController controller)
        {

        }

        public void Exit(StoneScorpionController controller) { }
    }
}
