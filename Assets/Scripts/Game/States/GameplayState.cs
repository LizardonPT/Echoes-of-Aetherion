using EchoesOfEtherion.StateMachine;
using UnityEngine;

namespace EchoesOfEtherion.Game.States
{
    public class GameplayState : IState<GameMaster>
    {
        public void Enter(GameMaster master) { }

        public void Exit(GameMaster master) { }

        public void Update(GameMaster master)
        {
            if (master.InputReader.PauseInputPressed)
            {
                master.PauseGame();
            }
        }

        public void FixedUpdate(GameMaster master)
        {

        }
    }
}
