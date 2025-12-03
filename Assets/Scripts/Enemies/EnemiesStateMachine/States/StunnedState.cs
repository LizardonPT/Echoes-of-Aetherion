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
        [SerializeField] private TimerCondition timer;

        public override void OnEnter()
        {
            timer.SetDuration(agent.StunTime);
            timer.StartTimer();
        }

        public override void OnFixedUpdate()
        {
            Vector2 velocity = agent.RB.linearVelocity;
            float speed = velocity.magnitude;

            if (speed < 0.01f)
            {
                agent.RB.linearVelocity = Vector2.zero;
                return;
            }

            float drop = speed * 15f * Time.fixedDeltaTime;
            float newSpeed = Mathf.Max(speed - drop, 0);
            agent.RB.linearVelocity = velocity * (newSpeed / speed);
        }
    }
}