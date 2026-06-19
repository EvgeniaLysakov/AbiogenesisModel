namespace AbiogenesisModel.Lib.Interfaces;

public interface IEvent<in TContext>
    where TContext : IContext
{
    bool TryExecute(TContext context);
}