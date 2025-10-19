namespace EchoesOfEtherion.StateMachine
{
    public interface IState<T> where T : class
    {
        void Enter(T entity);
        void Update(T entity);
        void FixedUpdate(T entity);
        void Exit(T entity);
    }
}
