using EchoesOfEtherion.StateMachine;
using UnityEngine;

namespace EchoesOfEtherion.Game.States
{
    public class GamePauseState : IState<GameMaster>
    {
        public void Enter(GameMaster master) { }

        public void Update(GameMaster master)
        {
            if (master.InputReader.PauseInputPressed)
            {
                master.ResumeGame();
            }
        }

        public void Exit(GameMaster master)
        {
            master.ResumeGame();
        }

        public void FixedUpdate(GameMaster master) { }
    }
}
