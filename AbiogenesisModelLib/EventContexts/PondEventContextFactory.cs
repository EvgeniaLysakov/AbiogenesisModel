using AbiogenesisModel.Lib.Attributes;
using AbiogenesisModel.Lib.Model;
using AbiogenesisModel.Lib.Model.Controllers;

namespace AbiogenesisModel.Lib.EventContexts;

[Service]
public class PondEventContextFactory(
    PondController pondController,
    StratumController stratumController,
    StratumPopulationController stratumPopulationController)
{
    public PondEventContext Create(ExternalEnvironment externalEnvironment, Pond pond)
    {
        return new PondEventContext(externalEnvironment, pond, pondController, stratumController, stratumPopulationController);
    }
}