using UnityEngine;
using EchoesOfEtherion.Extentions;
using EchoesOfEtherion.StateMachine;
using EchoesOfEtherion.Player.Components;
using UnityEngine.EventSystems;
using EchoesOfEtherion.Game.Utils;

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
            controller.Inventory.UpdateInput();

            if (controller.PlayerInput.AttackInputPressed)
            {
                if (!UIHelpers.IsPointerOverInteractableUI())
                {
                    controller.SpellCaster.CastSelectedSpell();
                }   
            }
        }

        public void FixedUpdate(PlayerController controller)
        {
            controller.Movement.UpdateMovement(controller.PlayerInput.MovementInput);
        }

        public void Exit(PlayerController controller) { }
    }
}
