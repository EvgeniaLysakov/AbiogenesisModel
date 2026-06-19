namespace AbiogenesisModel.Lib.Interfaces;

public interface ISlowEvent<in TContext> : IEvent<TContext>
    where TContext : IContext
{
}