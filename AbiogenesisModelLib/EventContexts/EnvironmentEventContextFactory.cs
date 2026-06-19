using AbiogenesisModel.Lib.Attributes;
using AbiogenesisModel.Lib.Model;
using AbiogenesisModel.Lib.Model.Controllers;

namespace AbiogenesisModel.Lib.EventContexts;

[Service]
public class EnvironmentEventContextFactory(ExternalEnvironmentController externalEnvironmentController)
{
    public EnvironmentEventContext Create(ExternalEnvironment externalEnvironment)
    {
        return new EnvironmentEventContext(externalEnvironment, externalEnvironmentController);
    }
}