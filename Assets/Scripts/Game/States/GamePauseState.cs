using EchoesOfAetherion.StateMachine;
using UnityEngine;
namespace EchoesOfAetherion.Game.States
{
    public class GamePauseState : IState<GameMaster>
    {
        public void Enter(GameMaster master)
        {
            Time.timeScale = 0f;
            master.Player?.SetCanTick(false);
            master.MenuController?.ShowPauseMenu();
        }

        public void Update(GameMaster master)
        {
            if (master.InputReader.PauseInputPressed)
            {
                master.StateMachine.ChangeState<GameplayState>();
            }
        }

        public void Exit(GameMaster master)
        {
            Time.timeScale = 1f;
            master.Player?.SetCanTick(true);
            master.MenuController?.HidePauseMenu();
        }

        public void FixedUpdate(GameMaster master) { }
    }
}