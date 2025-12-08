using EchoesOfEtherion.CameraUtils;
using EchoesOfEtherion.Player.Components;
using EchoesOfEtherion.Game.StateMachine;
using UnityEngine;

namespace EchoesOfEtherion.Player.States
{
    public class PlayerNoClipState : IState<PlayerController>
    {
        private CameraFollow cameraFollow;
        private Collider2D collider;
        public static float speed = 200;

        public void Enter(PlayerController controller)
        {
            collider = controller.GetComponent<Collider2D>();
            collider.enabled = false;

            controller.Movement.RB.linearVelocity = Vector2.zero;
            controller.Movement.RB.Sleep();

            cameraFollow = CameraController.Instance.GetComponent<CameraFollow>();
            cameraFollow.SetHasLimits(false);
        }

        public void Update(PlayerController controller)
        {
            controller.Animator.UpdateWalkAnimation(controller.PlayerInput.MovementInput, controller.LookDirection);
        }

        public void FixedUpdate(PlayerController controller)
        {
            Vector2 moveInput = controller.PlayerInput.MovementInput;
            if (moveInput.magnitude > 1e-5f)
            {
                moveInput.x = Mathf.Abs(moveInput.x) < 1e-5f ? 0 : moveInput.x;
                moveInput.y = Mathf.Abs(moveInput.y) < 1e-5f ? 0 : moveInput.y;

                Vector2 moveFoce = speed * Time.fixedDeltaTime * moveInput.normalized;
                Vector2 origin = controller.transform.position;

                Vector2 finalForce = origin + moveFoce;

                controller.transform.position = finalForce;
            }
        }

        public void Exit(PlayerController controller)
        {
            collider.enabled = true;
            controller.Movement.RB.WakeUp();
            cameraFollow.SetHasLimits(true);
        }
    }
}