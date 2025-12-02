using EchoesOfEtherion.Extentions;
using EchoesOfEtherion.StateMachine;
using UnityEngine;
using EchoesOfEtherion.Player.Components;

namespace EchoesOfEtherion.Player.States
{
    public class PlayerStunState : IState<PlayerController>
    {
        private float timer = 0;

        public void Enter(PlayerController controller)
        {
            timer = controller.StunTime;
        }

        public void Update(PlayerController controller)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                controller.StateMachine.ChangeState<PlayerRoamingState>();
            }

            controller.Animator.UpdateWalkAnimation(Vector2.zero, controller.LookDirection);
        }

        public void FixedUpdate(PlayerController controller) { }

        public void Exit(PlayerController controller) { }
    }
}
