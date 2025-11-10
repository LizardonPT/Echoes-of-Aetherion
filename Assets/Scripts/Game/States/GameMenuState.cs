using EchoesOfEtherion.StateMachine;
using UnityEngine;

namespace EchoesOfEtherion.Game.States
{
    public class GameMenuState : IState<GameMaster>
    {
        public void Enter(GameMaster master)
        {
            master.Log("Entering Menu State");

            // Menu-specific setup
            master.SetTimeScale(1f); // Keep time running for animations
            master.SetTickPaused(true); // But pause game logic
            master.SetGamePaused(true);
        }

        public void Exit(GameMaster master)
        {
            master.Log("Exiting Menu State");
        }

        public void Update(GameMaster master) { }
        public void FixedUpdate(GameMaster master) { }
    }
}
