using UnityEngine;
using EchoesOfEtherion.Extentions;
using EchoesOfEtherion.StateMachine;
using EchoesOfEtherion.Player.Components;

namespace EchoesOfEtherion.Player.States
{
    public class PlayerMovingState : IState<PlayerController>
    {
        public void Enter(PlayerController controller) { }

        public void Update(PlayerController controller)
        {
            controller.Animator.UpdateAnimation(controller.PlayerInput.MovementInput, controller.LookDirection);

            if (!controller.Movement.IsMoving)
            {
                controller.StateMachine.ChangeState<PlayerIdleState>();
            }

            if (controller.PlayerInput.InteractInputPressed)
            {
                controller.Interactor.InteractInput();
            }
        }

        public void FixedUpdate(PlayerController controller)
        {
            controller.Movement.UpdateMovement(controller.PlayerInput.MovementInput);
        }

        public void Exit(PlayerController controller) { }
    }
}
