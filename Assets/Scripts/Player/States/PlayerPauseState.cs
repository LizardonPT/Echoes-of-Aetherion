using EchoesOfAetherion.Extentions;
using EchoesOfAetherion.StateMachine;
using UnityEngine;
using EchoesOfAetherion.Player.Components;

namespace EchoesOfAetherion.Player.States
{
    public class PlayerPauseMenuState : IState<PlayerController>
    {
        public void Enter(PlayerController controller) { }

        public void Update(PlayerController controller)
        {
            if (controller.PlayerInput.PauseInputPressed)
            {
                Time.timeScale = 1f;
                controller.MenuController?.TogglePause();
                controller.StateMachine.ChangeState<PlayerIdleState>();
            }
        }

        public void FixedUpdate(PlayerController controller) { }

        public void Exit(PlayerController controller) { }
    }
}