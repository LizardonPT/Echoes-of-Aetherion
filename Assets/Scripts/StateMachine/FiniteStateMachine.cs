using System;
using System.Collections.Generic;

namespace EchoesOfAetherion.StateMachine
{
    public class FiniteStateMachine<T> where T : class
    {
        private readonly T entity;
        private IState<T> currentState;
        private readonly Dictionary<Type, IState<T>> states = new();

        public FiniteStateMachine(T entity)
        {
            this.entity = entity ?? throw new ArgumentNullException(nameof(entity));
        }

        public void AddState<U>(IState<T> state) where U : IState<T>
        {
            states[typeof(U)] = state;
        }

        public void ChangeState<U>() where U : IState<T>
        {
            currentState?.Exit(entity);

            if (states.TryGetValue(typeof(U), out IState<T> newState))
            {
                currentState = newState;
                currentState.Enter(entity);
            }
        }

        public void Update()
        {
            currentState?.Update(entity);
        }

        public void FixedUpdate()
        {
            currentState?.FixedUpdate(entity);
        }

        public Type GetCurrentStateType() => currentState?.GetType();
    }
}
