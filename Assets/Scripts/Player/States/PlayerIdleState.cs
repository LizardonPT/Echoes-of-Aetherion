using EchoesOfEtherion.Extentions;
using EchoesOfEtherion.StateMachine;
using UnityEngine;
using EchoesOfEtherion.Player.Components;
using UnityEngine.EventSystems;

namespace EchoesOfEtherion.Player.States
{
    public class PlayerIdleState : IState<PlayerController>
    {
        public void Enter(PlayerController controller) { }

        public void Update(PlayerController controller)
        {
            controller.Animator.UpdateWalkAnimation(Vector2.zero, controller.LookDirection);

            if (controller.PlayerInput.MovementInput.magnitude > 1e-5f)
            {
                controller.StateMachine.ChangeState<PlayerMovingState>();
            }

            if (controller.PlayerInput.InteractInputPressed)
            {
                controller.Interactor.InteractInput();
            }

            controller.Inventory.UpdateInput();

            if (controller.PlayerInput.AttackInputPressed)
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    controller.SpellCaster.CastSelectedSpell();
                }
                    
            }
        }

        public void FixedUpdate(PlayerController controller)
        {
            controller.Movement.UpdateMovement(Vector2.zero);
        }

        public void Exit(PlayerController controller) { }
    }
}
