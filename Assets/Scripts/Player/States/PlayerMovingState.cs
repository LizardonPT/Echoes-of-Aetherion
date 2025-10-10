using EchoesOfAetherion.StateMachine;
using PlayerController = EchoesOfAetherion.Player.Components.PlayerController;

namespace EchoesOfAetherion.Player.States
{
    public class PlayerMovingState : IState<PlayerController>
    {
        public void Enter(PlayerController controller) { }

        public void Update(PlayerController controller)
        {
            controller.Animator.UpdateAnimation(controller.Data.MovementInput, controller.Data.LookDirection);

            if (!controller.Data.IsMoving)
            {
                controller.StateMachine.ChangeState<PlayerIdleState>();
            }
        }

        public void FixedUpdate(PlayerController controller)
        {
            controller.Movement.UpdateMovement(controller.Data.MovementInput);
        }

        public void Exit(PlayerController controller) { }
    }
}