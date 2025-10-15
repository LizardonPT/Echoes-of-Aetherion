using EchoesOfAetherion.Extentions;
using EchoesOfAetherion.StateMachine;
using UnityEngine;
using EchoesOfAetherion.Player.Components;

namespace EchoesOfAetherion.Player.States
{
    public class PlayerIdleState : IState<PlayerController>
    {
        public void Enter(PlayerController controller) { }

        public void Update(PlayerController controller)
        {
            controller.Animator.UpdateAnimation(Vector2.zero, controller.LookDirection);

            if (controller.PlayerInput.MovementInput.magnitude > 1e-5f)
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