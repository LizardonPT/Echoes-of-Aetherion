using EchoesOfEtherion.Extentions;
using EchoesOfEtherion.Game.StateMachine;
using UnityEngine;
using EchoesOfEtherion.Player.Components;
using UnityEngine.EventSystems;
using EchoesOfEtherion.Game.Utils;
using EchoesOfEtherion.DeveloperConsole;

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

                controller.Inventory.UpdateInput();

                if (controller.PlayerInput.AttackInputPressed)
                {
                    if (!UIHelpers.IsPointerOverInteractableUI())
                    {
                        controller.SpellCaster.CastSelectedSpell();
                    }
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
