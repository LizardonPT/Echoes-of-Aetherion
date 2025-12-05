using System.Collections.Generic;
using EchoesOfEtherion.Enemies.Core;
using EchoesOfEtherion.Enemies.EnemiesStateMachine.Conditions;
using UnityEngine;

namespace EchoesOfEtherion.Enemies.EnemiesStateMachine.States
{
    public abstract class BaseState : MonoBehaviour
    {
        [Header("State Transitions")]
        [SerializeField] protected List<StateTransition> transitions = new();

        [Header("State Settings")]
        [SerializeField] protected bool canBeInterrupted = true;

        protected StateMachine stateMachine;
        protected Agent agent;
        protected float stateTimer;
        protected bool isActive;

        protected bool finished;

        public bool Finished
        {
            get
            {
                return canBeInterrupted || finished;
            }
        }

        public List<StateTransition> Transitions => transitions;

        public void Initialize(StateMachine sm)
        {
            stateMachine = sm;
            agent = sm.Agent;
            OnInitialize();
        }

        protected virtual void OnInitialize() { }

        public virtual void OnEnter()
        {
            isActive = true;
            stateTimer = 0f;
            enabled = true;
        }

        public virtual void OnUpdate()
        {
            stateTimer += Time.deltaTime;
        }

        public virtual void OnFixedUpdate() { }

        public virtual void OnExit()
        {
            isActive = false;
            enabled = false;
            finished = false;
        }

#if UNITY_EDITOR
        protected virtual void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying || !isActive) return;

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(agent.transform.position + Vector3.up * 1f, 0.3f);
        }
#endif
    }
}
