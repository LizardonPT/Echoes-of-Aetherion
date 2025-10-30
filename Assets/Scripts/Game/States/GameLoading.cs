using EchoesOfEtherion.StateMachine;
using UnityEngine;

namespace EchoesOfEtherion.Game.States
{
    public class GameLoadingState : IState<GameMaster>
    {
        public void Enter(GameMaster master)
        {
            // During loading, we want time to progress normally for async operations
            // but we don't want normal gameplay to run
            Time.timeScale = 1f;
            master.SetPausedWithoutTimeScale(true);
            master.PauseMenu?.HidePauseMenu();
        }

        public void Exit(GameMaster master)
        {
            // Return to appropriate time scale based on target state
            // This will be handled by the state we're transitioning to
        }

        public void Update(GameMaster master) { }

        public void FixedUpdate(GameMaster master) { }
    }
}
