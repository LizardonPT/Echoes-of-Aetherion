using System;
using System.Collections.Generic;

namespace EchoesOfEtherion.StateMachine
{
    public class FiniteStateMachine<T> where T : class
    {
        private readonly T entity;
        public IState<T> CurrentState { get; private set; }
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
            CurrentState?.Exit(entity);

            if (states.TryGetValue(typeof(U), out IState<T> newState))
            {
                CurrentState = newState;
                CurrentState.Enter(entity);
            }
        }

        public void Update()
        {
            CurrentState?.Update(entity);
        }

        public void FixedUpdate()
        {
            CurrentState?.FixedUpdate(entity);
        }

        public Type GetCurrentStateType() => CurrentState?.GetType();
    }
}
