using System;

namespace EchoesOfAetherion.StateMachine
{
    public class Transition<T> where T : class
    {
        public Action Actions { get; }
        public IState<T> TargetState { get; }
        private readonly Func<bool> condition;

        public bool IsTriggered()
        {
            return condition();
        }

        public Transition(Func<bool> condition, Action actions, IState<T> targetState)
        {
            this.condition = condition;
            Actions = actions;
            TargetState = targetState;
        }
    }
}
