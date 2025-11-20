using EchoesOfEtherion.StateMachine;
using UnityEngine;

namespace EchoesOfEtherion.Game.States
{
    public class GamePauseState : IState<GameMaster>
    {
        public void Enter(GameMaster master)
        {
            master.Log("Entering Pause State");

            // Pause the game
            master.SetTimeScale(0f);
            master.SetTickPaused(true);
            master.SetGamePaused(true);
        }

        public void Exit(GameMaster master)
        {
            master.Log("Exiting Pause State");
        }

        public void Update(GameMaster master)
        {
            if (master.InputReader.PauseInputPressed)
            {
                master.RequestResume();
            }
        }

        public void FixedUpdate(GameMaster master) { }
    }
}
