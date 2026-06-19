using AbiogenesisModel.Lib.Interfaces;
using AbiogenesisModel.Lib.Model;
using AbiogenesisModel.Lib.Model.Controllers;

namespace AbiogenesisModel.Lib.EventContexts;

public class PondEventContext(
    ExternalEnvironment externalEnvironment,
    Pond pond,
    PondController pondController,
    StratumController stratumController,
    StratumPopulationController stratumPopulationController) : IContext
{
    public ExternalEnvironment ExternalEnvironment { get; } = externalEnvironment;

    public Pond Pond { get; } = pond;

    public PondController PondController { get; } = pondController;

    public StratumController StratumController { get; } = stratumController;

    public StratumPopulationController StratumPopulationController { get; } = stratumPopulationController;
}