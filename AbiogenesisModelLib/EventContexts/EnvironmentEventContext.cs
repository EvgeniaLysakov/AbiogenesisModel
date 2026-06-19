using AbiogenesisModel.Lib.Interfaces;
using AbiogenesisModel.Lib.Model;
using AbiogenesisModel.Lib.Model.Controllers;

namespace AbiogenesisModel.Lib.EventContexts;

public class EnvironmentEventContext(ExternalEnvironment externalEnvironment, ExternalEnvironmentController externalEnvironmentController) : IContext
{
    public ExternalEnvironment ExternalEnvironment { get; } = externalEnvironment;
    public ExternalEnvironmentController ExternalEnvironmentController { get; } = externalEnvironmentController;
}