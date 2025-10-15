using EchoesOfAetherion.StateMachine;
using UnityEngine;
namespace EchoesOfAetherion.Game.States
{
    public class GameplayState : IState<GameMaster>
    {
        public void Enter(GameMaster master)
        {
            master.ResumeGame();
            master.MenuController?.HidePauseMenu();
        }

        public void Exit(GameMaster master)
        {
        }

        public void Update(GameMaster master)
        {
            if (master.InputReader.PauseInputPressed)
            {
                master.StateMachine.ChangeState<GamePauseState>();
            }
        }

        public void FixedUpdate(GameMaster master)
        {
            
        }
    }
}