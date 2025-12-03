using System.Collections.Generic;
using EchoesOfEtherion.Enemies.Core;
using UnityEngine;

namespace EchoesOfEtherion.Enemies.EnemiesStateMachine.States
{
    public abstract class BaseState : MonoBehaviour
    {
        [Header("State Transitions")]
        [SerializeField] protected List<StateTransition> transitions = new();

        protected StateMachine stateMachine;
        protected Agent agent;

        public List<StateTransition> Transitions => transitions;

        public void Initialize(StateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
            this.agent = stateMachine.Agent;
        }

        public virtual void OnEnter() { enabled = true; }
        public virtual void OnExit() { enabled = false; }
        public virtual void OnUpdate() { }
        public virtual void OnFixedUpdate() { }

        protected void ChangeState<T>() where T : BaseState
        {
            stateMachine.ChangeState(typeof(T));
        }
    }
}
