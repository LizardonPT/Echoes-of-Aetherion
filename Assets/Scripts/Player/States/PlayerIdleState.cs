using EchoesOfAetherion.StateMachine;
using UnityEngine;
using PlayerController = EchoesOfAetherion.Player.Components.PlayerController;

namespace EchoesOfAetherion.Player.States
{
    public class PlayerIdleState : IState<PlayerController>
    {
        public void Enter(PlayerController character) { }

        public void Update(PlayerController character)
        {
            character.Animator.UpdateAnimation(Vector2.zero, character.Data.LookDirection);

            if (character.Data.IsMoving)
            {
                character.StateMachine.ChangeState<PlayerMovingState>();
            }
        }

        public void FixedUpdate(PlayerController character)
        {
            character.Movement.UpdateMovement(Vector2.zero);
        }

        public void Exit(PlayerController character) { }
    }
}