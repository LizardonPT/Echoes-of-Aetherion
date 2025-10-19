using EchoesOfEtherion.StateMachine;
using UnityEngine;
namespace EchoesOfEtherion.Game.States
{
    public class GamePauseState : IState<GameMaster>
    {
        public void Enter(GameMaster master)
        {
            master.PauseGame();
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
            master.ResumeGame();
            master.MenuController?.HidePauseMenu();
        }

        public void FixedUpdate(GameMaster master) { }
    }
}
