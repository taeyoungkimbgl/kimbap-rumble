public abstract class BaseState<TContext>
{
    public virtual void OnEnter(TContext context, BaseState<TContext> prevState) { }
    public virtual void OnUpdate(TContext context) { }
    public virtual void OnExit(TContext context, BaseState<TContext> nextState) { }
}
