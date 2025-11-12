using EchoesOfEtherion.Extentions;
using EchoesOfEtherion.StateMachine;
using UnityEngine;
using EchoesOfEtherion.Player.Components;

namespace EchoesOfEtherion.Player.States
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

            if (controller.PlayerInput.InteractInputPressed)
            {
                controller.Interactor.InteractInput();
            }

            CheckSpellInputs(controller);
        }

        private void CheckSpellInputs(PlayerController controller)
        {
            if (controller.PlayerInput.SpellSlotInputPressed)
            {
                controller.SpellCaster.CastSpell(1, controller.LookDirection);
            }
        }

        public void FixedUpdate(PlayerController controller)
        {
            controller.Movement.UpdateMovement(Vector2.zero);
        }

        public void Exit(PlayerController controller) { }
    }
}
