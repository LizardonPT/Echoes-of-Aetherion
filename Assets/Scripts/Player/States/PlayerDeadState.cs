using EchoesOfEtherion.Extentions;
using EchoesOfEtherion.StateMachine;
using UnityEngine;
using EchoesOfEtherion.Player.Components;

namespace EchoesOfEtherion.Player.States
{
    public class PlayerDeadState : IState<PlayerController>
    {
        public void Enter(PlayerController controller)
        {
            controller.Animator.Die();
            controller.Movement.RB.linearVelocity = Vector2.zero;
        }

        //todo: Restart game with Input.
        public void Update(PlayerController controller) { }

        public void FixedUpdate(PlayerController controller) { }

        public void Exit(PlayerController controller) { }
    }
}
