using EchoesOfEtherion.StateMachine;
using UnityEngine;

namespace EchoesOfEtherion.Game.States
{
    public class GameplayState : IState<GameMaster>
    {
        public void Enter(GameMaster master)
        {
            master.Log("Entering Gameplay State");

            // Ensure game is running normally
            master.SetTimeScale(1f);
            master.SetTickPaused(false);
            master.SetGamePaused(false);
            master.HidePauseMenu();

            master.InvokeGameplayStarted();
        }

        public void Exit(GameMaster master)
        {
            master.Log("Exiting Gameplay State");
        }

        public void Update(GameMaster master)
        {
            if (master.InputReader.PauseInputPressed)
            {
                master.RequestPause();
            }
        }

        public void FixedUpdate(GameMaster master) { }
    }
}
