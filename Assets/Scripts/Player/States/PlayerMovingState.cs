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
            controller.Animator.UpdateWalkAnimation(controller.PlayerInput.MovementInput, controller.LookDirection);

            if (!controller.Movement.IsMoving)
            {
                controller.StateMachine.ChangeState<PlayerIdleState>();
            }

            if (controller.PlayerInput.InteractInputPressed)
            {
                controller.Interactor.InteractInput();
            }
            CheckSpellInputs(controller);
        }

        private void CheckSpellInputs(PlayerController controller)
        {
            if (controller.PlayerInput.Slot1InputPressed)
            {
                controller.SpellCaster.CastSpell(1);
            }
            else if (controller.PlayerInput.Slot2InputPressed)
            {
                controller.SpellCaster.CastSpell(2);
            }
            else if (controller.PlayerInput.Slot3InputPressed)
            {
                controller.SpellCaster.CastSpell(3);
            }
            else if (controller.PlayerInput.Slot4InputPressed)
            {
                controller.SpellCaster.CastSpell(4);
            }
            else if (controller.PlayerInput.Slot5InputPressed)
            {
                controller.SpellCaster.CastSpell(5);
            }
            else if (controller.PlayerInput.Slot6InputPressed)
            {
                controller.SpellCaster.CastSpell(6);
            }
        }


        public void FixedUpdate(PlayerController controller)
        {
            controller.Movement.UpdateMovement(controller.PlayerInput.MovementInput);
        }

        public void Exit(PlayerController controller) { }
    }
}
