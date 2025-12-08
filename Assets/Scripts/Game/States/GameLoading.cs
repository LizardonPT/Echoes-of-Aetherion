using EchoesOfEtherion.Game.StateMachine;
using UnityEngine;

namespace EchoesOfEtherion.Game.States
{
    public class GameLoadingState : IState<GameMaster>
    {
        public void Enter(GameMaster master)
        {
            master.Log("Entering Loading State");

            // Keep time running for async operations but pause game logic
            master.SetTimeScale(1f);
            master.SetTickPaused(true);
            master.SetGamePaused(true);

            master.InvokeLoadingStarted();
        }

        public void Exit(GameMaster master)
        {
            master.Log("Exiting Loading State");
        }

        public void Update(GameMaster master) { }
        public void FixedUpdate(GameMaster master) { }
    }
}
