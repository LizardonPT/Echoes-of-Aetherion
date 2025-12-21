// PlayerRoamingState.cs (updated)
using EchoesOfEtherion.Extentions;
using EchoesOfEtherion.Game.StateMachine;
using UnityEngine;
using EchoesOfEtherion.Player.Components;
using UnityEngine.EventSystems;
using EchoesOfEtherion.Game.Utils;
using EchoesOfEtherion.DeveloperConsole;
using EchoesOfEtherion.Spells;

namespace EchoesOfEtherion.Player.States
{
    public class PlayerRoamingState : IState<PlayerController>
    {
        public void Enter(PlayerController controller) { }

        public void Update(PlayerController controller)
        {
            Vector2 moveInput = ConsoleController.Instance.IsOpen ?
                Vector2.zero : controller.PlayerInput.MovementInput;

            controller.Animator.UpdateWalkAnimation(moveInput, controller.LookDirection);

            // Check input only if the command is not opened.
            if (!ConsoleController.Instance.IsOpen)
            {
                if (controller.PlayerInput.InteractInputPressed)
                {
                    controller.Interactor.InteractInput();
                }

                if (controller.PlayerInput.HealInputPressed)
                {
                    controller.SpellCaster.CastSpell(controller.SpellInventory.HealSpell);
                }

                if (controller.PlayerInput.BasicProjectileInputPressed)
                {
                    controller.SpellCaster.CastSpell(controller.SpellInventory.BasicProjectileSpell);
                }

                if (controller.PlayerInput.BlinkInputPressed)
                {
                    controller.SpellCaster.CastSpell(controller.SpellInventory.BlinkSpell);
                }

                if (controller.PlayerInput.SpellSlot1InputPressed)
                {
                    controller.SpellCaster.CastSlotSpell(0);
                }

                if (controller.PlayerInput.SpellSlot2InputPressed)
                {
                    controller.SpellCaster.CastSlotSpell(1);
                }

                if (controller.PlayerInput.SpellSlot3InputPressed)
                {
                    controller.SpellCaster.CastSlotSpell(2);
                }

                if (controller.PlayerInput.SpellSlot4InputPressed)
                {
                    controller.SpellCaster.CastSlotSpell(3);
                }

                if (controller.PlayerInput.NextSpellSetInputPressed)
                {
                    controller.SpellInventory.NextSpellSet();
                }

                if (controller.PlayerInput.PreviousSpellSetInputPressed)
                {
                    controller.SpellInventory.PreviousSpellSet();
                }
            }
        }

        public void FixedUpdate(PlayerController controller)
        {
            if (ConsoleController.Instance.IsOpen)
            {
                controller.Movement.UpdateMovement(Vector2.zero);
                return;
            }
            Vector2 movementInput = controller.PlayerInput.MovementInput;
            movementInput.x = Mathf.Abs(movementInput.x) < 1e-5f ? 0 : movementInput.x;
            movementInput.y = Mathf.Abs(movementInput.y) < 1e-5f ? 0 : movementInput.y;

            controller.Movement.UpdateMovement(movementInput);
        }

        public void Exit(PlayerController controller) { }
    }
}
