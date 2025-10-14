using UnityEngine;
using EchoesOfAetherion.Extentions;
using EchoesOfAetherion.StateMachine;
using EchoesOfAetherion.Player.Components;

namespace EchoesOfAetherion.Player.States
{
    public class PlayerMovingState : IState<PlayerController>
    {
        public void Enter(PlayerController controller) { }

        public void Update(PlayerController controller)
        {
            if (controller.MoveInputAction.action.TryReadValue(out Vector2 movementInput))
            {
                controller.Animator.UpdateAnimation(movementInput, controller.LookDirection);
            }

            if (!controller.Movement.IsMoving)
            {
                controller.StateMachine.ChangeState<PlayerIdleState>();
            }
        }

        public void FixedUpdate(PlayerController controller)
        {
            if (controller.MoveInputAction.action.TryReadValue(out Vector2 movementInput))
                controller.Movement.UpdateMovement(movementInput);
        }

        public void Exit(PlayerController controller) { }
    }
}