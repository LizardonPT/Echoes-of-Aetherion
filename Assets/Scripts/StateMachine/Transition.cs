using System;

namespace EchoesOfAetherion.StateMachine
{
    public class Transition<T> where T : class
    {
        public Action OnTransition { get; }
        public IState<T> TargetState { get; }
        private readonly Func<bool> condition;

        public bool IsTriggered()
        {
            return condition();
        }

        public Transition(Func<bool> condition, Action actions, IState<T> targetState)
        {
            this.condition = condition ?? throw new ArgumentNullException(nameof(condition));
            OnTransition = actions ?? throw new ArgumentNullException(nameof(actions));
            TargetState = targetState ?? throw new ArgumentNullException(nameof(targetState));
        }
    }
}
