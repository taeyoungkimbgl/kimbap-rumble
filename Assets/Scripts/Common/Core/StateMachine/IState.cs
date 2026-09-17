namespace Scripts.Common.Core.StateMachine
{
    public interface IState<TStateId>
    {
        TStateId StateId { get; }
        void Enter();
        void Update();
        void Exit();
    }
}
