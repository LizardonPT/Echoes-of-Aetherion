using System;
using System.Collections.Generic;
using EchoesOfEtherion.Enemies.Core;
using EchoesOfEtherion.Enemies.EnemiesStateMachine.States;
using UnityEngine;

namespace EchoesOfEtherion.Enemies.EnemiesStateMachine
{
    public class StateMachine : MonoBehaviour
    {
        [Header("State Configuration")]
        [SerializeField] private BaseState initialState;

        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = true;
        [SerializeField] private Color debugTextColor = Color.yellow;

        private Agent agent;
        private BaseState currentState;
        private Dictionary<Type, BaseState> stateCache = new();

        public BaseState CurrentState => currentState;
        public Agent Agent => agent;

        private void Awake()
        {
            agent = GetComponent<Agent>();
            CacheAllStates();
            InitializeConditions();

            if (initialState != null)
            {
                ChangeState(initialState.GetType());
            }
        }

        private void CacheAllStates()
        {
            var states = GetComponents<BaseState>();
            foreach (var state in states)
            {
                stateCache[state.GetType()] = state;
                state.Initialize(this);
                state.enabled = false; // Start disabled
            }
        }

        private void InitializeConditions()
        {
            // Initialize all conditions on all states
            foreach (var state in stateCache.Values)
            {
                foreach (var transition in state.Transitions)
                {
                    if (transition.Condition != null)
                    {
                        transition.Condition.Initialize(agent);
                    }
                }
            }
        }

        public void ChangeState(Type newStateType)
        {
            if (!stateCache.TryGetValue(newStateType, out var newState))
            {
                Debug.LogError($"State {newStateType.Name} not found!");
                return;
            }

            if (newState == currentState) return;

#if UNITY_EDITOR
            if (showDebugInfo)
            {
                Debug.Log($"{gameObject.name}: {currentState?.GetType().Name ?? "None"} → {newStateType.Name}", gameObject);
            }
#endif

            currentState?.OnExit();
            currentState = newState;
            currentState.OnEnter();
        }

        private void Update()
        {
            currentState?.OnUpdate();

            // Check transitions for current state
            foreach (var transition in currentState?.Transitions ?? new List<StateTransition>())
            {
                bool conditionMet = transition.Condition.IsMet();
                if (transition.Condition != null && conditionMet)
                {
                    ChangeState(transition.TargetState.GetType());
                    break;
                }
            }
        }

        private void FixedUpdate()
        {
            currentState?.OnFixedUpdate();
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!showDebugInfo || !Application.isPlaying) return;

            // Draw current state name above enemy
            string stateName = currentState?.GetType().Name ?? "No State";
            UnityEditor.Handles.Label(
                transform.position + Vector3.up * 2f,
                $"State: {stateName}",
                new GUIStyle()
                {
                    normal = new GUIStyleState() { textColor = debugTextColor },
                    fontSize = 12,
                    fontStyle = FontStyle.Bold
                }
            );

            // Draw transitions
            if (currentState != null)
            {
                foreach (var transition in currentState.Transitions)
                {
                    if (transition.Condition != null && transition.TargetState != null)
                    {
                        Gizmos.color = transition.Condition.IsMet() ? Color.green : Color.gray;
                        Gizmos.DrawLine(
                            transform.position + Vector3.up * 1.5f,
                            transform.position + Vector3.up * 1.8f
                        );
                    }
                }
            }
        }
#endif
    }
}