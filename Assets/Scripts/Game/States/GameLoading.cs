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
        }

        public void Exit(GameMaster master)
        {
            master.SetPausedWithoutTimeScale(false);
        }

        public void Update(GameMaster master) { }

        public void FixedUpdate(GameMaster master) { }
    }
}
