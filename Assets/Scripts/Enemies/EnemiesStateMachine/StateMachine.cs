using System;
using System.Collections.Generic;
using EchoesOfEtherion.Enemies.Core;
using EchoesOfEtherion.Enemies.EnemiesStateMachine.Conditions;
using EchoesOfEtherion.Enemies.EnemiesStateMachine.States;
using EchoesOfEtherion.Game;
using UnityEngine;

namespace EchoesOfEtherion.Enemies.EnemiesStateMachine
{
    [RequireComponent(typeof(Agent))]
    public class StateMachine : TickRegistor
    {
        [Header("State Configuration")]
        [SerializeField] private BaseState initialState;

        private Agent agent;
        private BaseState currentState;
        private readonly Dictionary<Type, BaseState> stateCache = new();

        private readonly List<BaseCondition> conditions = new();
        private readonly List<BaseState> states = new();

        public BaseState CurrentState => currentState;
        public Agent Agent => agent;

        private void Awake()
        {
            agent = GetComponent<Agent>();
            CacheAllStates();
            InitializeConditions();

            if (initialState != null)
            {
                currentState = initialState;
                currentState.OnEnter();
            }
        }

        private void CacheAllStates()
        {
            var stateComponents = GetComponents<BaseState>();
            foreach (var state in stateComponents)
            {
                stateCache[state.GetType()] = state;
                states.Add(state);
                state.Initialize(this);
                state.enabled = false;
            }
        }

        private void InitializeConditions()
        {
            // Initialize all conditions
            foreach (BaseCondition condition in GetComponentsInChildren<BaseCondition>())
            {
                condition.Initialize(agent);
                conditions.Add(condition);
            }
        }

        public void ChangeState(BaseState newState)
        {
            if (!states.Contains(newState))
            {
                Debug.LogError($"State {newState.GetType().Name} not found!");
                return;
            }

            if (newState == currentState) return;

            currentState.OnExit();
            currentState = newState;
            currentState.OnEnter();
        }

        public override void Tick()
        {
            currentState?.OnUpdate();
            conditions.ForEach(c => c.OnUpdate());

            foreach (var transition in currentState?.Transitions ?? new List<StateTransition>())
            {
                if (!currentState.Finished) continue;

                bool conditionMet = transition.Condition.IsMet();
                if (transition.Condition != null && conditionMet)
                {
                    ChangeState(transition.TargetState);
                    break;
                }
            }
        }

        public override void FixedTick()
        {
            currentState?.OnFixedUpdate();
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;

            // Draw current state name above enemy
            string stateName = currentState?.GetType().Name ?? "No State";
            UnityEditor.Handles.Label(
                transform.position + Vector3.up * 2f,
                $"State: {stateName}",
                new GUIStyle()
                {
                    normal = new GUIStyleState() { textColor = Color.yellow },
                    fontSize = 12,
                    fontStyle = FontStyle.Bold
                }
            );
        }
#endif
    }
}
