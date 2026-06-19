using AbiogenesisModel.Lib.EventContexts;

namespace AbiogenesisModel.Lib.Interfaces;

public interface IEnvironmentEvent : ISlowEvent<EnvironmentEventContext>
{
}