namespace AbiogenesisModel.Lib.Interfaces;

public interface IFastEvent<in TContext> : IEvent<TContext>
    where TContext : IContext
{
    int GetVelocity(TContext context);
}