using EchoesOfEtherion.Enemies.Core;
using EchoesOfEtherion.Enemies.EnemiesStateMachine;
using EchoesOfEtherion.Enemies.EnemiesStateMachine.Conditions;
using EchoesOfEtherion.Enemies.EnemiesStateMachine.States;
using UnityEngine;
using UnityEngine.AI;

namespace EchoesOfEtherion.Enemies.StoneScorpion.States
{
    public class StunnedState : BaseState
    {
        private float timer;

        protected override void OnInitialize()
        {

        }

        public override void OnEnter()
        {
            base.OnEnter();

            timer = agent.StunTime;
        }

        public override void OnUpdate()
        {
            if (finished) return;
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                finished = true;
                return;
            }
        }
    }
}