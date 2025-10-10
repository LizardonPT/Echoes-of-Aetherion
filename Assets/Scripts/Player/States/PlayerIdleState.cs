using EchoesOfAetherion.StateMachine;
using UnityEngine;
using PlayerController = EchoesOfAetherion.Player.Components.PlayerController;

namespace EchoesOfAetherion.Player.States
{
    public class PlayerIdleState : IState<PlayerController>
    {
        public void Enter(PlayerController controller) { }

        public void Update(PlayerController controller)
        {
            controller.Animator.UpdateAnimation(Vector2.zero, controller.Data.LookDirection);

            if (controller.Data.IsMoving)
            {
                controller.StateMachine.ChangeState<PlayerMovingState>();
            }
        }

        public void FixedUpdate(PlayerController controller)
        {
            controller.Movement.UpdateMovement(Vector2.zero);
        }

        public void Exit(PlayerController controller) { }
    }
}