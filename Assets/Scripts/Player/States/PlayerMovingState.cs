using EchoesOfAetherion.StateMachine;
using PlayerController = EchoesOfAetherion.Player.Components.PlayerController;

namespace EchoesOfAetherion.Player.States
{
    public class PlayerMovingState : IState<PlayerController>
    {
        public void Enter(PlayerController character) { }

        public void Update(PlayerController character)
        {
            character.Animator.UpdateAnimation(character.Data.MovementInput, character.Data.LookDirection);

            if (!character.Data.IsMoving)
            {
                character.StateMachine.ChangeState<PlayerIdleState>();
            }
        }

        public void FixedUpdate(PlayerController character)
        {
            character.Movement.UpdateMovement(character.Data.MovementInput);
        }

        public void Exit(PlayerController character) { }
    }
}